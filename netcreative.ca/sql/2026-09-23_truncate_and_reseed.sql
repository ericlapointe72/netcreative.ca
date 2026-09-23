/*
    CONTACT / LOGIN / SUBSCRIBE — wipe and reseed
    ================================================
    Empties all three tables and resets AUTO_NUMBER back to 1 on each.
    TRUNCATE TABLE does both in one statement (unlike DELETE, it resets
    the identity seed automatically) and is minimally logged, so it's
    the right tool here rather than DELETE + DBCC CHECKIDENT.

    USERS and OPENING are intentionally NOT touched by this script.

    This is IRREVERSIBLE without a backup. Confirmed: backup taken.
    Run this in SSMS against NETCREATIVE, ONE STEP AT A TIME.
*/

USE [NETCREATIVE]
GO


-- ============================================================
-- STEP 1 — informational: row counts before wiping, for the record.
-- ============================================================

SELECT 'CONTACT' AS TableName, COUNT(*) AS RowsBefore FROM [dbo].[CONTACT]
UNION ALL
SELECT 'LOGIN', COUNT(*) FROM [dbo].[LOGIN]
UNION ALL
SELECT 'SUBSCRIBE', COUNT(*) FROM [dbo].[SUBSCRIBE];
GO


-- ============================================================
-- STEP 2 — truncate. Empties each table and resets AUTO_NUMBER
-- to start at 1 again on the next insert.
-- ============================================================

TRUNCATE TABLE [dbo].[CONTACT];
TRUNCATE TABLE [dbo].[LOGIN];
TRUNCATE TABLE [dbo].[SUBSCRIBE];
GO


-- ============================================================
-- STEP 3 — sanity check: all three should show 0 rows, and the
-- next AUTO_NUMBER inserted into each should be 1.
-- ============================================================

SELECT 'CONTACT' AS TableName, COUNT(*) AS RowsAfter, IDENT_CURRENT('CONTACT') AS LastSeed FROM [dbo].[CONTACT]
UNION ALL
SELECT 'LOGIN', COUNT(*), IDENT_CURRENT('LOGIN') FROM [dbo].[LOGIN]
UNION ALL
SELECT 'SUBSCRIBE', COUNT(*), IDENT_CURRENT('SUBSCRIBE') FROM [dbo].[SUBSCRIBE];
GO
