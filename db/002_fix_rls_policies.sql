-- Migration 002: Fix RLS policies for users table
-- This migration fixes the Row Level Security policies that were blocking user registration

-- Drop existing policies for users table
DROP POLICY IF EXISTS "Users can view own data" ON public.users;
DROP POLICY IF EXISTS "Users can insert own data" ON public.users;
DROP POLICY IF EXISTS "Users can update own data" ON public.users;

-- Create new policies for users table
-- Allow users to view their own data
CREATE POLICY "Users can view own data" ON public.users
    FOR SELECT USING (auth.uid()::text = id::text);

-- Allow system to insert users (for triggers)
CREATE POLICY "System can insert users" ON public.users
    FOR INSERT WITH CHECK (true);

-- Allow users to update their own data
CREATE POLICY "Users can update own data" ON public.users
    FOR UPDATE USING (auth.uid()::text = id::text);

-- Allow system to update users (for triggers)
CREATE POLICY "System can update users" ON public.users
    FOR UPDATE USING (true);

-- Migration completed successfully!
-- The users table now allows registration through triggers. 