/*
    LOGIN table — modernize DATE/TIME storage, drop dead PASSWORD column
    ======================================================================
    Today: [DATE] char(10)  holding strings like '2026/09/22'
           [TIME] char(11)  holding strings like '14:23:05' OR '2:23:05 PM'
           (same format story as the old CONTACT table — see
           2026-09-22_contact_date_time_types.sql)
           [PASSWORD] varchar(20) — always inserted as an empty string by
           login.aspx.cs/Site.Master.cs, never read back anywhere. Pure
           dead weight, safe to drop.
    After:  [DATE] date
            [TIME] time(0)
            [PASSWORD] column removed

    USERS table — drop the per-user permission flags, add a real key
    ======================================================================
    CONTACT/LOGIN/NAVIGATION/OPENING/SUBSCRIBER/USERS/VISITOR gated which
    admin.aspx tabs a given user could see. That per-user permission model
    is being removed — any logged-in admin now sees every tab, gated only
    by Session["role"] == "admin" (already enforced in admin.aspx.cs).
    NAVIGATION/VISITOR were already dead (their admin tabs were removed
    earlier this project); the other five are being retired along with
    the whole permission system.
    Also: USER_NAME had no PRIMARY KEY/UNIQUE constraint at all, even
    though every lookup/update/delete in the app assumes it's unique.
    Step 6 adds one, after checking there's nothing that would violate it.

    Run this in SSMS against NETCREATIVE, ONE STEP AT A TIME — read the
    comment before each step. Do not run the whole file blindly.
    Take a backup of the USERS/LOGIN tables (or the whole DB) before
    Step 3 and Step 6.
*/

USE [NETCREATIVE]
GO


-- ============================================================
-- STEP 1 — add two new columns to LOGIN and populate them from
-- the old string columns. TRY_CONVERT returns NULL instead of
-- throwing if a row's text doesn't parse, so this step is
-- non-destructive and safe to run even if some old rows are
-- malformed.
-- ============================================================

ALTER TABLE [dbo].[LOGIN] ADD [DATE_NEW] date NULL;
ALTER TABLE [dbo].[LOGIN] ADD [TIME_NEW] time(0) NULL;
GO

UPDATE [dbo].[LOGIN]
SET [DATE_NEW] = TRY_CONVERT(date, [DATE], 111),   -- style 111 = yyyy/mm/dd
    [TIME_NEW] = TRY_CONVERT(time(0), [TIME]);      -- handles both '14:23:05' and '2:23:05 PM'
GO


-- ============================================================
-- STEP 2 — check for rows that failed to convert (should return
-- zero rows). If this returns anything, open those specific rows
-- and fix [DATE]/[TIME] by hand before continuing to Step 3 —
-- do NOT proceed to Step 3 while this still returns rows.
-- ============================================================

SELECT [AUTO_NUMBER], [DATE], [TIME], [DATE_NEW], [TIME_NEW]
FROM [dbo].[LOGIN]
WHERE [DATE_NEW] IS NULL OR [TIME_NEW] IS NULL;
GO


-- ============================================================
-- STEP 3 — once Step 2 returns zero rows: drop the old DATE/TIME
-- columns plus the dead PASSWORD column, rename DATE_NEW/TIME_NEW
-- to take their place, and make them NOT NULL to match the
-- original constraint.
-- ============================================================

ALTER TABLE [dbo].[LOGIN] ALTER COLUMN [DATE_NEW] date NOT NULL;
ALTER TABLE [dbo].[LOGIN] ALTER COLUMN [TIME_NEW] time(0) NOT NULL;
GO

ALTER TABLE [dbo].[LOGIN] DROP COLUMN [DATE];
ALTER TABLE [dbo].[LOGIN] DROP COLUMN [TIME];
ALTER TABLE [dbo].[LOGIN] DROP COLUMN [PASSWORD];
GO

EXEC sp_rename 'dbo.LOGIN.DATE_NEW', 'DATE', 'COLUMN';
EXEC sp_rename 'dbo.LOGIN.TIME_NEW', 'TIME', 'COLUMN';
GO


-- ============================================================
-- STEP 4 — sanity check: confirm LOGIN's new column types and
-- that PASSWORD is gone.
-- ============================================================

SELECT COLUMN_NAME, DATA_TYPE, DATETIME_PRECISION
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'LOGIN'
ORDER BY ORDINAL_POSITION;
GO


-- ============================================================
-- STEP 5 — drop the retired permission-flag columns from USERS.
-- ============================================================

ALTER TABLE [dbo].[USERS] DROP COLUMN
    [CONTACT], [LOGIN], [NAVIGATION], [OPENING], [SUBSCRIBER], [USERS], [VISITOR];
GO


-- ============================================================
-- STEP 6 — add a real key on USER_NAME. First check for
-- duplicates (should return zero rows) — do NOT proceed to the
-- ALTER TABLE while this returns anything; resolve the duplicates
-- by hand first.
-- ============================================================

SELECT [USER_NAME], COUNT(*) AS DUPLICATE_COUNT
FROM [dbo].[USERS]
GROUP BY [USER_NAME]
HAVING COUNT(*) > 1;
GO

ALTER TABLE [dbo].[USERS] ADD CONSTRAINT [PK_USERS] PRIMARY KEY CLUSTERED ([USER_NAME]);
GO


-- ============================================================
-- STEP 7 — sanity check: confirm USERS' final column list.
-- ============================================================

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'USERS'
ORDER BY ORDINAL_POSITION;
GO
