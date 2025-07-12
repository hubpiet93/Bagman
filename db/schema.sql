-- Database schema for Bagman betting system
-- Supabase PostgreSQL

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

-- Indexes for better performance
CREATE INDEX idx_table_members_user_id ON public.table_members(user_id);
CREATE INDEX idx_table_members_table_id ON public.table_members(table_id);
CREATE INDEX idx_matches_table_id ON public.matches(table_id);
CREATE INDEX idx_matches_datetime ON public.matches(match_datetime);
CREATE INDEX idx_bets_user_id ON public.bets(user_id);
CREATE INDEX idx_bets_match_id ON public.bets(match_id);
CREATE INDEX idx_pools_match_id ON public.pools(match_id);
CREATE INDEX idx_user_stats_user_id ON public.user_stats(user_id);
CREATE INDEX idx_user_stats_table_id ON public.user_stats(table_id);

-- Row Level Security (RLS) policies
ALTER TABLE public.users ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.tables ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.table_members ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.matches ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.bets ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.pools ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.pool_winners ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.user_stats ENABLE ROW LEVEL SECURITY;

-- Basic RLS policies (to be expanded based on authentication requirements)
-- Users can only see their own data
CREATE POLICY "Users can view own data" ON public.users
    FOR SELECT USING (auth.uid()::text = id::text);

-- Table members can view table data if they are members
CREATE POLICY "Table members can view table data" ON public.tables
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.table_members 
            WHERE table_id = id AND user_id = auth.uid()::uuid
        )
    );

-- Table admins can modify table data
CREATE POLICY "Table admins can modify table data" ON public.tables
    FOR ALL USING (
        EXISTS (
            SELECT 1 FROM public.table_members 
            WHERE table_id = id AND user_id = auth.uid()::uuid AND is_admin = true
        )
    );

-- Users can view matches for tables they belong to
CREATE POLICY "Users can view matches for their tables" ON public.matches
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.table_members 
            WHERE table_id = table_id AND user_id = auth.uid()::uuid
        )
    );

-- Users can view their own bets
CREATE POLICY "Users can view own bets" ON public.bets
    FOR SELECT USING (user_id = auth.uid()::uuid);

-- Users can modify their own bets before match starts
CREATE POLICY "Users can modify own bets before match starts" ON public.bets
    FOR ALL USING (
        user_id = auth.uid()::uuid AND
        NOT EXISTS (
            SELECT 1 FROM public.matches 
            WHERE id = match_id AND started = true
        )
    );

-- Users can view pools for their tables
CREATE POLICY "Users can view pools for their tables" ON public.pools
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.matches m
            JOIN public.table_members tm ON m.table_id = tm.table_id
            WHERE m.id = match_id AND tm.user_id = auth.uid()::uuid
        )
    );

-- Users can view their own stats
CREATE POLICY "Users can view own stats" ON public.user_stats
    FOR SELECT USING (user_id = auth.uid()::uuid); 