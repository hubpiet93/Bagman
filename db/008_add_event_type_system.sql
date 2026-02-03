-- Migration: Add EventType system and SuperAdmin role
-- Date: 2026-02-01
-- Description: Przeprojektowanie zarządzania meczami - oddzielenie meczy od stołów przez EventType

-- 1. Add is_super_admin to users table
ALTER TABLE users
ADD COLUMN is_super_admin BOOLEAN NOT NULL DEFAULT FALSE;

CREATE INDEX idx_users_is_super_admin ON users(is_super_admin);

-- 2. Create event_types table
CREATE TABLE event_types (
    id UUID PRIMARY KEY,
    code VARCHAR(100) NOT NULL UNIQUE,
    name VARCHAR(255) NOT NULL,
    start_date TIMESTAMP NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX uk_event_types_code ON event_types(code);
CREATE INDEX idx_event_types_is_active ON event_types(is_active);

-- 3. Add event_type_id to tables
ALTER TABLE tables
ADD COLUMN event_type_id UUID;

-- Temporarily allow NULL for event_type_id during migration
-- You'll need to populate this with actual EventType IDs before adding NOT NULL constraint

-- 4. Change matches.table_id to event_type_id
-- First, drop the existing foreign key constraint
ALTER TABLE matches
DROP CONSTRAINT IF EXISTS fk_matches_tables;

-- Rename the column
ALTER TABLE matches
RENAME COLUMN table_id TO event_type_id;

-- Drop old index
DROP INDEX IF EXISTS idx_matches_table_id;

-- Create new index
CREATE INDEX idx_matches_event_type_id ON matches(event_type_id);

-- 5. Add foreign key constraints with RESTRICT to prevent deletion of EventTypes with dependencies
ALTER TABLE tables
ADD CONSTRAINT fk_tables_event_types 
    FOREIGN KEY (event_type_id) 
    REFERENCES event_types(id) 
    ON DELETE RESTRICT;

ALTER TABLE matches
ADD CONSTRAINT fk_matches_event_types 
    FOREIGN KEY (event_type_id) 
    REFERENCES event_types(id) 
    ON DELETE RESTRICT;

-- 6. Create index on tables.event_type_id
CREATE INDEX idx_tables_event_type_id ON tables(event_type_id);

-- IMPORTANT NOTES:
-- Before applying this migration in production:
-- 1. Create at least one EventType entry
-- 2. Update all existing tables to have event_type_id set
-- 3. Then add NOT NULL constraint:
--    ALTER TABLE tables ALTER COLUMN event_type_id SET NOT NULL;
--
-- Example EventType insert:
-- INSERT INTO event_types (id, code, name, start_date, is_active, created_at)
-- VALUES (gen_random_uuid(), 'DEFAULT_2026', 'Default Event 2026', NOW(), TRUE, NOW());
--
-- Example update existing tables:
-- UPDATE tables SET event_type_id = (SELECT id FROM event_types WHERE code = 'DEFAULT_2026' LIMIT 1);
