-- Complete migration for Bagman betting system
-- Supabase PostgreSQL migration - FINAL VERSION
-- Run this in Supabase SQL Editor
-- This migration includes all tables but NO triggers and minimal RLS policies

-- ========================================
-- 1. SCHEMA CREATION
-- ========================================

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Users table (extends Supabase auth.users)
CREATE TABLE public.users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    login VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    is_active BOOLEAN DEFAULT TRUE
);

-- Tables (gaming tables)
CREATE TABLE public.tables (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    max_players INTEGER NOT NULL CHECK (max_players > 0),
    stake DECIMAL(10,2) NOT NULL CHECK (stake >= 0),
    created_by UUID NOT NULL REFERENCES public.users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    is_secret_mode BOOLEAN DEFAULT FALSE -- for "tajemniczy typer" mode
);

-- Table members (users belonging to tables)
CREATE TABLE public.table_members (
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    table_id UUID NOT NULL REFERENCES public.tables(id) ON DELETE CASCADE,
    is_admin BOOLEAN DEFAULT FALSE,
    joined_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    PRIMARY KEY (user_id, table_id)
);

-- Matches
CREATE TABLE public.matches (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    table_id UUID NOT NULL REFERENCES public.tables(id) ON DELETE CASCADE,
    country_1 VARCHAR(100) NOT NULL,
    country_2 VARCHAR(100) NOT NULL,
    match_datetime TIMESTAMP WITH TIME ZONE NOT NULL,
    result VARCHAR(10), -- e.g., "2:1", "0:0"
    status VARCHAR(20) DEFAULT 'scheduled' CHECK (status IN ('scheduled', 'in_progress', 'finished')),
    started BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Bets (user predictions)
CREATE TABLE public.bets (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    match_id UUID NOT NULL REFERENCES public.matches(id) ON DELETE CASCADE,
    prediction VARCHAR(10) NOT NULL, -- e.g., "2:1", "0:0"
    edited_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(user_id, match_id)
);

-- Pools (prize pools for matches)
CREATE TABLE public.pools (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    match_id UUID NOT NULL REFERENCES public.matches(id) ON DELETE CASCADE,
    amount DECIMAL(10,2) NOT NULL DEFAULT 0,
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'won', 'rollover', 'expired')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Pool winners (many-to-many relationship)
CREATE TABLE public.pool_winners (
    pool_id UUID NOT NULL REFERENCES public.pools(id) ON DELETE CASCADE,
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    amount_won DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (pool_id, user_id)
);

-- User statistics per table
CREATE TABLE public.user_stats (
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    table_id UUID NOT NULL REFERENCES public.tables(id) ON DELETE CASCADE,
    matches_played INTEGER DEFAULT 0,
    bets_placed INTEGER DEFAULT 0,
    pools_won INTEGER DEFAULT 0,
    total_won DECIMAL(10,2) DEFAULT 0,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    PRIMARY KEY (user_id, table_id)
);

-- ========================================
-- 2. INDEXES FOR PERFORMANCE
-- ========================================

CREATE INDEX idx_table_members_user_id ON public.table_members(user_id);
CREATE INDEX idx_table_members_table_id ON public.table_members(table_id);
CREATE INDEX idx_matches_table_id ON public.matches(table_id);
CREATE INDEX idx_matches_datetime ON public.matches(match_datetime);
CREATE INDEX idx_bets_user_id ON public.bets(user_id);
CREATE INDEX idx_bets_match_id ON public.bets(match_id);
CREATE INDEX idx_pools_match_id ON public.pools(match_id);
CREATE INDEX idx_user_stats_user_id ON public.user_stats(user_id);
CREATE INDEX idx_user_stats_table_id ON public.user_stats(table_id);

-- ========================================
-- 3. MINIMAL RLS SETUP
-- ========================================

-- Disable RLS on users table completely (controlled from application)
ALTER TABLE public.users DISABLE ROW LEVEL SECURITY;

-- Enable RLS on other tables but with minimal policies
ALTER TABLE public.tables ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.table_members ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.matches ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.bets ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.pools ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.pool_winners ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.user_stats ENABLE ROW LEVEL SECURITY;

-- ========================================
-- 4. MINIMAL RLS POLICIES
-- ========================================

-- Grant all permissions to authenticated users on users table
GRANT ALL ON public.users TO authenticated;
GRANT USAGE ON SCHEMA public TO authenticated;

-- Grant all permissions to postgres (for admin operations)
GRANT ALL ON public.users TO postgres;

-- Basic policies for other tables (can be enhanced later)
-- Tables: anyone can view, only authenticated can create
CREATE POLICY "Anyone can view tables" ON public.tables FOR SELECT USING (true);
CREATE POLICY "Authenticated users can create tables" ON public.tables FOR INSERT WITH CHECK (auth.role() = 'authenticated');

-- Table members: basic access
CREATE POLICY "Table members can view members" ON public.table_members FOR SELECT USING (true);
CREATE POLICY "Authenticated users can join tables" ON public.table_members FOR INSERT WITH CHECK (auth.role() = 'authenticated');

-- Matches: basic access
CREATE POLICY "Anyone can view matches" ON public.matches FOR SELECT USING (true);
CREATE POLICY "Authenticated users can create matches" ON public.matches FOR INSERT WITH CHECK (auth.role() = 'authenticated');

-- Bets: users can manage their own bets
CREATE POLICY "Users can view own bets" ON public.bets FOR SELECT USING (user_id = auth.uid()::uuid);
CREATE POLICY "Users can insert own bets" ON public.bets FOR INSERT WITH CHECK (auth.uid()::uuid = user_id);
CREATE POLICY "Users can update own bets" ON public.bets FOR UPDATE USING (user_id = auth.uid()::uuid);

-- Pools: basic access
CREATE POLICY "Anyone can view pools" ON public.pools FOR SELECT USING (true);

-- Pool winners: basic access
CREATE POLICY "Anyone can view pool winners" ON public.pool_winners FOR SELECT USING (true);

-- User stats: users can view their own stats
CREATE POLICY "Users can view own stats" ON public.user_stats FOR SELECT USING (user_id = auth.uid()::uuid);

-- ========================================
-- 5. SAMPLE DATA FOR TESTING
-- ========================================

-- Insert sample users
INSERT INTO public.users (id, login, email) VALUES 
    ('550e8400-e29b-41d4-a716-446655440000', 'admin', 'admin@bagman.com'),
    ('550e8400-e29b-41d4-a716-446655440001', 'testuser', 'test@bagman.com')
ON CONFLICT (id) DO NOTHING;

-- Insert sample table
INSERT INTO public.tables (id, name, password_hash, max_players, stake, created_by) VALUES 
    ('550e8400-e29b-41d4-a716-446655440002', 'Mistrzostwa Świata 2026', '$2a$10$test', 10, 50.00, '550e8400-e29b-41d4-a716-446655440000')
ON CONFLICT (id) DO NOTHING;

-- Insert sample table members
INSERT INTO public.table_members (user_id, table_id, is_admin) VALUES 
    ('550e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440002', true),
    ('550e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440002', false)
ON CONFLICT (user_id, table_id) DO NOTHING;

-- ========================================
-- 6. MIGRATION COMPLETE
-- ========================================

-- Migration completed successfully!
-- The database is now ready for the Bagman betting system.
-- 
-- Key features of this migration:
-- - All tables created with proper relationships
-- - No triggers (everything controlled from application)
-- - RLS disabled on users table (no permission issues)
-- - Minimal RLS policies on other tables
-- - Sample data included for testing
-- - All necessary indexes for performance 