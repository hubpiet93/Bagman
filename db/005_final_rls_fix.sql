-- Migration 005: Final RLS fix - removes all existing policies and recreates them
-- This migration completely removes all existing policies and recreates them properly

-- Step 1: Drop all existing triggers and functions
DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;
DROP FUNCTION IF EXISTS public.handle_new_user();

-- Step 2: Drop ALL existing RLS policies on users table (with IF EXISTS to avoid errors)
DROP POLICY IF EXISTS "Users can view own profile" ON public.users;
DROP POLICY IF EXISTS "Users can update own profile" ON public.users;
DROP POLICY IF EXISTS "Enable insert for authenticated users only" ON public.users;
DROP POLICY IF EXISTS "Enable update for users based on id" ON public.users;
DROP POLICY IF EXISTS "Enable delete for users based on id" ON public.users;
DROP POLICY IF EXISTS "System can insert users" ON public.users;
DROP POLICY IF EXISTS "Users can delete own profile" ON public.users;

-- Step 3: Disable RLS temporarily
ALTER TABLE public.users DISABLE ROW LEVEL SECURITY;

-- Step 4: Create a new function with proper security context
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
    -- Insert user into public.users table
    INSERT INTO public.users (id, login, email, created_at, is_active)
    VALUES (
        NEW.id, 
        COALESCE(NEW.raw_user_meta_data->>'login', NEW.email), 
        NEW.email,
        NOW(),
        true
    );
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

-- Step 5: Grant necessary permissions
GRANT USAGE ON SCHEMA public TO postgres;
GRANT ALL ON public.users TO postgres;
GRANT EXECUTE ON FUNCTION public.handle_new_user() TO postgres;

-- Step 6: Create the trigger
CREATE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();

-- Step 7: Re-enable RLS
ALTER TABLE public.users ENABLE ROW LEVEL SECURITY;

-- Step 8: Create new RLS policies that allow system operations
-- Policy for system inserts (trigger) - this allows the trigger to insert users
CREATE POLICY "System can insert users" ON public.users
    FOR INSERT WITH CHECK (true);

-- Policy for users to view their own profile
CREATE POLICY "Users can view own profile" ON public.users
    FOR SELECT USING (auth.uid() = id);

-- Policy for users to update their own profile
CREATE POLICY "Users can update own profile" ON public.users
    FOR UPDATE USING (auth.uid() = id);

-- Policy for users to delete their own profile
CREATE POLICY "Users can delete own profile" ON public.users
    FOR DELETE USING (auth.uid() = id);

-- Step 9: Grant necessary permissions to authenticated users
GRANT SELECT, UPDATE, DELETE ON public.users TO authenticated;
GRANT USAGE ON SCHEMA public TO authenticated;

-- Migration completed successfully!
-- All existing policies have been removed and new ones created.
-- The trigger now has proper permissions and RLS policies are correctly configured. 