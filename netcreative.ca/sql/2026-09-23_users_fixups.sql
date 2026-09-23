/*
    USERS table — production schema drift fixes
    =============================================
    The production USERS table (post-cleanup) turned out to differ from
    what local dev had:

    1. [PASSWORD] varchar(50) — too small. PasswordHasher.Hash() always
       produces exactly 83 characters ("PBKDF2$100000$<24-char salt>$<44-char
       hash>"). Any password save/change against this column would fail
       with a truncation error, or corrupt the hash if truncation were
       silently allowed. Needs to be varchar(100), matching local dev.
    2. [NEWSLETTER] bit NULL — leftover column not present in local dev
       and not referenced anywhere in the current codebase (confirmed by
       grep). Safe to drop.

    Run this in SSMS against NETCREATIVE, ONE STEP AT A TIME.
    Take a backup before Step 2 (only step that removes anything).
*/

USE [NETCREATIVE]
GO


-- ============================================================
-- STEP 1 — widen PASSWORD. Widening never truncates existing
-- data, so this is safe regardless of what's currently stored.
-- ============================================================

ALTER TABLE [dbo].[USERS] ALTER COLUMN [PASSWORD] varchar(100) NOT NULL;
GO


-- ============================================================
-- STEP 2 — drop the unused NEWSLETTER column.
-- ============================================================

ALTER TABLE [dbo].[USERS] DROP COLUMN [NEWSLETTER];
GO


-- ============================================================
-- STEP 3 — sanity check: confirm USERS' final column list.
-- ============================================================

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'USERS'
ORDER BY ORDINAL_POSITION;
GO
