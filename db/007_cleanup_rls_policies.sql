-- Migration 007: Cleanup RLS policies on users table
-- This migration removes all RLS policies from users table since RLS is disabled

-- Drop all existing RLS policies on users table
DROP POLICY IF EXISTS "System can update users" ON public.users;
DROP POLICY IF EXISTS "Users can update own data" ON public.users;
DROP POLICY IF EXISTS "Users can view own data" ON public.users;
DROP POLICY IF EXISTS "System can insert users" ON public.users;
DROP POLICY IF EXISTS "Users can view own profile" ON public.users;
DROP POLICY IF EXISTS "Users can update own profile" ON public.users;
DROP POLICY IF EXISTS "Users can delete own profile" ON public.users;
DROP POLICY IF EXISTS "Enable insert for authenticated users only" ON public.users;
DROP POLICY IF EXISTS "Enable update for users based on id" ON public.users;
DROP POLICY IF EXISTS "Enable delete for users based on id" ON public.users;

-- Verify that RLS is disabled
ALTER TABLE public.users DISABLE ROW LEVEL SECURITY;

-- Grant all permissions to authenticated users
GRANT ALL ON public.users TO authenticated;
GRANT USAGE ON SCHEMA public TO authenticated;

-- Grant all permissions to postgres (for admin operations)
GRANT ALL ON public.users TO postgres;

-- Migration completed successfully!
-- All RLS policies have been removed from users table.
-- RLS is disabled and all operations are controlled from the application. 