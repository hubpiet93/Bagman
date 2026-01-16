# Database Migration Instructions for Bagman

## Overview
This document provides instructions for setting up the database for the Bagman football betting system using SQL Server / Azure SQL and EF Core.

## Quick Start

### 1. Use the Complete Migration (Recommended)
The `complete_migration_final.sql` file contains everything you need in one migration:

```sql
```
- ✅ All tables with proper relationships
- ✅ No triggers (application-controlled)
- ✅ RLS disabled on users table (no permission issues)
- ✅ Minimal RLS policies on other tables
- ✅ Sample data for testing
- ✅ All necessary indexes

### 2. Alternative: Step-by-step Migration
If you prefer to run migrations step by step:

1. **Initial Schema**: Run `001_initial_migration.sql`
2. **Cleanup**: Run `007_cleanup_rls_policies.sql`

## Migration Files

### `complete_migration_final.sql` (RECOMMENDED)
- **Complete solution**: Everything in one file
- **No triggers**: Application controls all logic
- **Minimal RLS**: Users table has RLS disabled
- **Ready to use**: Includes sample data

### `001_initial_migration.sql`
- **Full schema**: All tables, triggers, and complex RLS policies
- **May cause issues**: Complex triggers and policies
- **Not recommended**: Use complete_migration_final.sql instead

### `007_cleanup_rls_policies.sql`
- **Cleanup script**: Removes triggers and disables RLS on users
- **Use with**: 001_initial_migration.sql if you want step-by-step

## Database Schema

### Core Tables
- `users` - User accounts
- `tables` - Betting tables/groups
- `table_members` - Users belonging to tables
- `matches` - Football matches
- `bets` - User predictions
- `pools` - Prize pools for matches
- `pool_winners` - Winners of pools
- `user_stats` - User statistics per table

### Key Features
- UUID primary keys
- Proper foreign key relationships
- Cascade deletes where appropriate
- Timestamps for auditing
- Check constraints for data validation

## Security Model

### Row Level Security (RLS)
- **Users table**: RLS disabled (application controls access)
- **Other tables**: Minimal RLS policies
- **Authentication**: Uses Supabase auth.uid()

### Permissions
- Authenticated users can access most data
- Users can only modify their own bets
- Table admins have additional privileges

## Testing

### Sample Data
The migration includes sample data:
- Admin user: `admin@bagman.com`
- Test user: `test@bagman.com`
- Sample table: "Mistrzostwa Świata 2026"

### API Testing
Use the `api-requests.http` file to test the API endpoints.

## Troubleshooting

### Common Issues

1. **RLS Violation Errors**
   - Solution: Use `complete_migration_final.sql` (RLS disabled on users)

2. **Duplicate Key Errors**
   - Check if user already exists before registration
   - Ensure proper error handling in application

3. **Permission Denied**
   - Verify Supabase service role key is configured
   - Check RLS policies are appropriate

### Verification Queries

```sql
-- Check if tables exist
SELECT table_name FROM information_schema.tables 
WHERE table_schema = 'public' AND table_name LIKE '%users%';

-- Check RLS status
SELECT schemaname, tablename, rowsecurity 
FROM pg_tables 
WHERE schemaname = 'public';

-- Check triggers
SELECT trigger_name, event_manipulation, event_object_table 
FROM information_schema.triggers 
WHERE trigger_schema = 'public';
```

## Next Steps

1. **Run the migration**: Use `complete_migration_final.sql`
2. **Configure Supabase**: Set up environment variables
3. **Test the API**: Use the provided HTTP requests
4. **Deploy**: Ready for production use

## Support

If you encounter issues:
1. Check the troubleshooting section
2. Verify Supabase configuration
3. Review application logs for detailed error messages 