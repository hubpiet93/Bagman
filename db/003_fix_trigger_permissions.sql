-- Migration 003: Fix trigger permissions and function security
-- This migration fixes the trigger permissions that were causing RLS violations

-- Drop existing trigger and function
DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;
DROP FUNCTION IF EXISTS public.handle_new_user();

-- Recreate the function with proper security context
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
    -- Use SECURITY DEFINER to bypass RLS
    INSERT INTO public.users (id, login, email)
    VALUES (NEW.id, COALESCE(NEW.raw_user_meta_data->>'login', NEW.email), NEW.email);
    RETURN NEW;
EXCEPTION
    WHEN unique_violation THEN
        -- If user already exists, just return
        RETURN NEW;
    WHEN OTHERS THEN
        -- Log the error but don't fail the auth.users insert
        RAISE WARNING 'Failed to create user in public.users: %', SQLERRM;
        RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Recreate the trigger
CREATE OR REPLACE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();

-- Grant necessary permissions to the function
GRANT USAGE ON SCHEMA public TO postgres;
GRANT ALL ON public.users TO postgres;

-- Migration completed successfully!
-- The trigger now has proper permissions to insert users. 