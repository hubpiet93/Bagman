-- Complete migration for Bagman betting system
-- PostgreSQL migration (originally targeted Supabase). Review and adapt for your SQL Server / Azure SQL environment if needed.
-- Users table

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
-- 3. FUNCTIONS AND TRIGGERS
-- ========================================

-- Function to create a new user in our users table when auth.users is created
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO public.users (id, login, email)
    VALUES (NEW.id, NEW.raw_user_meta_data->>'login', NEW.email);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Trigger to automatically create user in our users table
CREATE OR REPLACE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();

-- Function to calculate pool winners when match result is set
CREATE OR REPLACE FUNCTION public.calculate_pool_winners(match_uuid UUID)
RETURNS VOID AS $$
DECLARE
    match_result VARCHAR(10);
    pool_record RECORD;
    winner_count INTEGER := 0;
    amount_per_winner DECIMAL(10,2);
BEGIN
    -- Get match result
    SELECT result INTO match_result
    FROM public.matches
    WHERE id = match_uuid;
    
    IF match_result IS NULL THEN
        RAISE EXCEPTION 'Match result not set';
    END IF;
    
    -- Get pool for this match
    SELECT * INTO pool_record
    FROM public.pools
    WHERE match_id = match_uuid;
    
    IF pool_record IS NULL THEN
        RAISE EXCEPTION 'No pool found for this match';
    END IF;
    
    -- Count winners (users who predicted correctly)
    SELECT COUNT(*) INTO winner_count
    FROM public.bets
    WHERE match_id = match_uuid AND prediction = match_result;
    
    -- If no winners, rollover the pool
    IF winner_count = 0 THEN
        UPDATE public.pools
        SET status = 'rollover'
        WHERE match_id = match_uuid;
        RETURN;
    END IF;
    
    -- Calculate amount per winner
    amount_per_winner := pool_record.amount / winner_count;
    
    -- Insert winners
    INSERT INTO public.pool_winners (pool_id, user_id, amount_won)
    SELECT pool_record.id, user_id, amount_per_winner
    FROM public.bets
    WHERE match_id = match_uuid AND prediction = match_result;
    
    -- Update pool status
    UPDATE public.pools
    SET status = 'won'
    WHERE match_id = match_uuid;
    
    -- Update user statistics
    UPDATE public.user_stats
    SET 
        pools_won = pools_won + 1,
        total_won = total_won + amount_per_winner,
        updated_at = NOW()
    WHERE user_id IN (
        SELECT user_id
        FROM public.bets
        WHERE match_id = match_uuid AND prediction = match_result
    );
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Function to create pool when match is created
CREATE OR REPLACE FUNCTION public.create_match_pool()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO public.pools (match_id, amount)
    VALUES (NEW.id, 0);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Trigger to automatically create pool for new matches
CREATE OR REPLACE TRIGGER on_match_created
    AFTER INSERT ON public.matches
    FOR EACH ROW EXECUTE FUNCTION public.create_match_pool();

-- Function to update pool amount when bet is placed
CREATE OR REPLACE FUNCTION public.update_pool_amount()
RETURNS TRIGGER AS $$
DECLARE
    table_stake DECIMAL(10,2);
BEGIN
    -- Get table stake
    SELECT stake INTO table_stake
    FROM public.tables t
    JOIN public.matches m ON t.id = m.table_id
    WHERE m.id = NEW.match_id;
    
    -- Update pool amount
    UPDATE public.pools
    SET amount = amount + table_stake
    WHERE match_id = NEW.match_id;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Trigger to update pool when bet is placed
CREATE OR REPLACE TRIGGER on_bet_placed
    AFTER INSERT ON public.bets
    FOR EACH ROW EXECUTE FUNCTION public.update_pool_amount();

-- Function to get user dashboard data
CREATE OR REPLACE FUNCTION public.get_user_dashboard(user_uuid UUID)
RETURNS TABLE (
    table_id UUID,
    table_name VARCHAR(100),
    user_role VARCHAR(10),
    matches_count BIGINT,
    active_bets_count BIGINT,
    total_won DECIMAL(10,2)
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        t.id,
        t.name,
        CASE WHEN tm.is_admin THEN 'admin' ELSE 'member' END,
        COUNT(m.id),
        COUNT(b.id),
        COALESCE(us.total_won, 0)
    FROM public.tables t
    JOIN public.table_members tm ON t.id = tm.table_id
    LEFT JOIN public.matches m ON t.id = m.table_id
    LEFT JOIN public.bets b ON m.id = b.match_id AND b.user_id = user_uuid
    LEFT JOIN public.user_stats us ON t.id = us.table_id AND us.user_id = user_uuid
    WHERE tm.user_id = user_uuid
    GROUP BY t.id, t.name, tm.is_admin, us.total_won;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Function to get table dashboard data
CREATE OR REPLACE FUNCTION public.get_table_dashboard(table_uuid UUID, user_uuid UUID)
RETURNS TABLE (
    table_name VARCHAR(100),
    max_players INTEGER,
    current_players BIGINT,
    stake DECIMAL(10,2),
    matches_count BIGINT,
    user_bets_count BIGINT,
    user_total_won DECIMAL(10,2),
    is_admin BOOLEAN
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        t.name,
        t.max_players,
        COUNT(tm.user_id),
        t.stake,
        COUNT(m.id),
        COUNT(b.id),
        COALESCE(us.total_won, 0),
        tm.is_admin
    FROM public.tables t
    JOIN public.table_members tm ON t.id = tm.table_id
    LEFT JOIN public.matches m ON t.id = m.table_id
    LEFT JOIN public.bets b ON m.id = b.match_id AND b.user_id = user_uuid
    LEFT JOIN public.user_stats us ON t.id = us.table_id AND us.user_id = user_uuid
    WHERE t.id = table_uuid AND tm.user_id = user_uuid
    GROUP BY t.name, t.max_players, t.stake, tm.is_admin, us.total_won;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- ========================================
-- 4. ROW LEVEL SECURITY (RLS)
-- ========================================

-- Enable RLS on all tables
ALTER TABLE public.users ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.tables ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.table_members ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.matches ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.bets ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.pools ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.pool_winners ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.user_stats ENABLE ROW LEVEL SECURITY;

-- ========================================
-- 5. RLS POLICIES
-- ========================================

-- Users policies
CREATE POLICY "Users can view own data" ON public.users
    FOR SELECT USING (auth.uid()::text = id::text);

CREATE POLICY "Users can insert own data" ON public.users
    FOR INSERT WITH CHECK (auth.uid()::text = id::text);

CREATE POLICY "Users can update own data" ON public.users
    FOR UPDATE USING (auth.uid()::text = id::text);

-- Tables policies
CREATE POLICY "Table members can view table data" ON public.tables
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.table_members 
            WHERE table_id = id AND user_id = auth.uid()::uuid
        )
    );

CREATE POLICY "Users can create tables" ON public.tables
    FOR INSERT WITH CHECK (auth.uid()::uuid = created_by);

CREATE POLICY "Table admins can modify table data" ON public.tables
    FOR ALL USING (
        EXISTS (
            SELECT 1 FROM public.table_members 
            WHERE table_id = id AND user_id = auth.uid()::uuid AND is_admin = true
        )
    );

-- Table members policies
CREATE POLICY "Table members can view table members" ON public.table_members
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.table_members tm
            WHERE tm.table_id = table_id AND tm.user_id = auth.uid()::uuid
        )
    );

CREATE POLICY "Users can join tables" ON public.table_members
    FOR INSERT WITH CHECK (auth.uid()::uuid = user_id);

CREATE POLICY "Users can leave tables" ON public.table_members
    FOR DELETE USING (auth.uid()::uuid = user_id);

CREATE POLICY "Table admins can manage table members" ON public.table_members
    FOR ALL USING (
        EXISTS (
            SELECT 1 FROM public.table_members tm
            WHERE tm.table_id = table_id AND tm.user_id = auth.uid()::uuid AND tm.is_admin = true
        )
    );

-- Matches policies
CREATE POLICY "Users can view matches for their tables" ON public.matches
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.table_members 
            WHERE table_id = table_id AND user_id = auth.uid()::uuid
        )
    );

CREATE POLICY "Table admins can insert matches" ON public.matches
    FOR INSERT WITH CHECK (
        EXISTS (
            SELECT 1 FROM public.table_members tm
            WHERE tm.table_id = table_id AND tm.user_id = auth.uid()::uuid AND tm.is_admin = true
        )
    );

CREATE POLICY "Table admins can update matches before start" ON public.matches
    FOR UPDATE USING (
        EXISTS (
            SELECT 1 FROM public.table_members tm
            WHERE tm.table_id = table_id AND tm.user_id = auth.uid()::uuid AND tm.is_admin = true
        ) AND NOT started
    );

CREATE POLICY "Table admins can set match results" ON public.matches
    FOR UPDATE USING (
        EXISTS (
            SELECT 1 FROM public.table_members tm
            WHERE tm.table_id = table_id AND tm.user_id = auth.uid()::uuid AND tm.is_admin = true
        )
    );

CREATE POLICY "Table admins can delete matches before start" ON public.matches
    FOR DELETE USING (
        EXISTS (
            SELECT 1 FROM public.table_members tm
            WHERE tm.table_id = table_id AND tm.user_id = auth.uid()::uuid AND tm.is_admin = true
        ) AND NOT started
    );

-- Bets policies
CREATE POLICY "Users can view own bets" ON public.bets
    FOR SELECT USING (user_id = auth.uid()::uuid);

CREATE POLICY "Users can insert own bets" ON public.bets
    FOR INSERT WITH CHECK (auth.uid()::uuid = user_id);

CREATE POLICY "Users can modify own bets before match starts" ON public.bets
    FOR ALL USING (
        user_id = auth.uid()::uuid AND
        NOT EXISTS (
            SELECT 1 FROM public.matches 
            WHERE id = match_id AND started = true
        )
    );

CREATE POLICY "Table members can view all bets" ON public.bets
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.matches m
            JOIN public.table_members tm ON m.table_id = tm.table_id
            WHERE m.id = match_id AND tm.user_id = auth.uid()::uuid
        )
    );

-- Pools policies
CREATE POLICY "Users can view pools for their tables" ON public.pools
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.matches m
            JOIN public.table_members tm ON m.table_id = tm.table_id
            WHERE m.id = match_id AND tm.user_id = auth.uid()::uuid
        )
    );

CREATE POLICY "Table members can view pools" ON public.pools
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.matches m
            JOIN public.table_members tm ON m.table_id = tm.table_id
            WHERE m.id = match_id AND tm.user_id = auth.uid()::uuid
        )
    );

-- Pool winners policies
CREATE POLICY "Users can view own pool wins" ON public.pool_winners
    FOR SELECT USING (auth.uid()::uuid = user_id);

CREATE POLICY "Table members can view pool winners" ON public.pool_winners
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.pools p
            JOIN public.matches m ON p.match_id = m.id
            JOIN public.table_members tm ON m.table_id = tm.table_id
            WHERE p.id = pool_id AND tm.user_id = auth.uid()::uuid
        )
    );

-- User stats policies
CREATE POLICY "Users can view own stats" ON public.user_stats
    FOR SELECT USING (auth.uid()::uuid = user_id);

CREATE POLICY "Table members can view table stats" ON public.user_stats
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.table_members tm
            WHERE tm.table_id = table_id AND tm.user_id = auth.uid()::uuid
        )
    );

-- System policies for triggers
CREATE POLICY "System can update user stats" ON public.user_stats
    FOR UPDATE USING (true);

CREATE POLICY "System can insert user stats" ON public.user_stats
    FOR INSERT WITH CHECK (true);

CREATE POLICY "System can insert pool winners" ON public.pool_winners
    FOR INSERT WITH CHECK (true);

CREATE POLICY "System can update pools" ON public.pools
    FOR UPDATE USING (true);

-- ========================================
-- 6. SAMPLE DATA FOR TESTING
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
-- 7. MIGRATION COMPLETE
-- ========================================

-- Migration completed successfully!
-- The database is now ready for the Bagman betting system. 