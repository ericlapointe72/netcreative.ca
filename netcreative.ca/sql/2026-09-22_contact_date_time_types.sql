/*
    CONTACT table — modernize DATE/TIME storage
    ============================================
    Today: [DATE] char(10)  holding strings like '2026/09/22'
           [TIME] nchar(11) holding strings like '14:23:05' OR '2:23:05 PM'
           (format depended on the visitor's selected site language at
           submission time — fr-CA is 24h, the neutral "en" culture used
           by Global.SetCulture formats 12h with AM/PM)
    After:  [DATE] date
            [TIME] time(0)

    Run this in SSMS against NETCREATIVE, ONE STEP AT A TIME — read the
    comment before each step. Do not run the whole file blindly.
    Take a backup of the CONTACT table (or the whole DB) before Step 3.
*/

USE [NETCREATIVE]
GO


-- ============================================================
-- STEP 1 — add two new columns and populate them from the old
-- string columns. TRY_CONVERT returns NULL instead of throwing
-- if a row's text doesn't parse, so this step is non-destructive
-- and safe to run even if some old rows are malformed.
-- ============================================================

ALTER TABLE [dbo].[CONTACT] ADD [DATE_NEW] date NULL;
ALTER TABLE [dbo].[CONTACT] ADD [TIME_NEW] time(0) NULL;
GO

UPDATE [dbo].[CONTACT]
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
FROM [dbo].[CONTACT]
WHERE [DATE_NEW] IS NULL OR [TIME_NEW] IS NULL;
GO


-- ============================================================
-- STEP 3 — once Step 2 returns zero rows: drop the old columns,
-- rename the new ones to take their place, and make them NOT NULL
-- to match the original constraint.
-- ============================================================

ALTER TABLE [dbo].[CONTACT] ALTER COLUMN [DATE_NEW] date NOT NULL;
ALTER TABLE [dbo].[CONTACT] ALTER COLUMN [TIME_NEW] time(0) NOT NULL;
GO

ALTER TABLE [dbo].[CONTACT] DROP COLUMN [DATE];
ALTER TABLE [dbo].[CONTACT] DROP COLUMN [TIME];
GO

EXEC sp_rename 'dbo.CONTACT.DATE_NEW', 'DATE', 'COLUMN';
EXEC sp_rename 'dbo.CONTACT.TIME_NEW', 'TIME', 'COLUMN';
GO


-- ============================================================
-- STEP 4 — sanity check: confirm the new column types.
-- ============================================================

SELECT COLUMN_NAME, DATA_TYPE, DATETIME_PRECISION
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'CONTACT' AND COLUMN_NAME IN ('DATE', 'TIME');
GO
