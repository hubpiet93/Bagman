-- Migration 006: Simple solution - minimal RLS and no triggers
-- This migration removes all complex triggers and RLS policies
-- Everything will be controlled from the application

-- Step 1: Drop all existing triggers and functions
DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;
DROP FUNCTION IF EXISTS public.handle_new_user();

-- Step 2: Drop ALL existing RLS policies on users table
DROP POLICY IF EXISTS "Users can view own profile" ON public.users;
DROP POLICY IF EXISTS "Users can update own profile" ON public.users;
DROP POLICY IF EXISTS "Enable insert for authenticated users only" ON public.users;
DROP POLICY IF EXISTS "Enable update for users based on id" ON public.users;
DROP POLICY IF EXISTS "Enable delete for users based on id" ON public.users;
DROP POLICY IF EXISTS "System can insert users" ON public.users;
DROP POLICY IF EXISTS "Users can delete own profile" ON public.users;

-- Step 3: Disable RLS on users table completely
ALTER TABLE public.users DISABLE ROW LEVEL SECURITY;

-- Step 4: Grant all permissions to authenticated users
GRANT ALL ON public.users TO authenticated;
GRANT USAGE ON SCHEMA public TO authenticated;

-- Step 5: Grant all permissions to postgres (for admin operations)
GRANT ALL ON public.users TO postgres;

-- Migration completed successfully!
-- RLS is disabled on users table - all operations will be controlled from the application.
-- No triggers or complex policies - simple and straightforward approach. 