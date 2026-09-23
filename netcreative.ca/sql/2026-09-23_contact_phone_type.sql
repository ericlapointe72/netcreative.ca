/*
    CONTACT table — fix PHONE column type
    ======================================
    Today: [PHONE] nchar(20) NOT NULL — fixed-width, right-padded with
           spaces on every insert. It's the only nchar column in the
           table (everything else is varchar), and oversized: the
           client-side mask (phone-mask.js) always produces exactly
           14 characters, e.g. "(123) 456-7890", and TextBox_Phone has
           MaxLength="14" to match. The padding is invisible in most
           places but shows up as trailing whitespace in ContactLog.aspx,
           which prints "EMAIL | PHONE" with PHONE last on the line.
    After: [PHONE] varchar(14) NOT NULL

    Run this in SSMS against NETCREATIVE, ONE STEP AT A TIME — read the
    comment before each step. Take a backup of the CONTACT table (or the
    whole DB) before Step 2.
*/

USE [NETCREATIVE]
GO


-- ============================================================
-- STEP 1 — safety check: any row whose trimmed phone number is
-- longer than 14 characters would be truncated by Step 2. This
-- should return zero rows (the mask has capped input at 14 chars
-- since day one); if it returns anything, fix those rows by hand
-- before continuing.
-- ============================================================

SELECT [AUTO_NUMBER], [PHONE], LEN(RTRIM([PHONE])) AS TrimmedLength
FROM [dbo].[CONTACT]
WHERE LEN(RTRIM([PHONE])) > 14;
GO


-- ============================================================
-- STEP 2 — once Step 1 returns zero rows: narrow the column to
-- varchar(14) FIRST. Trimming while the column is still nchar(20)
-- doesn't work — nchar always re-pads whatever you assign back out
-- to its fixed width, so an RTRIM done before this ALTER gets
-- silently undone by the column itself (verified on the production
-- run of this script: the trim had no effect until the column was
-- already varchar).
-- ============================================================

ALTER TABLE [dbo].[CONTACT] ALTER COLUMN [PHONE] varchar(14) NOT NULL;
GO

UPDATE [dbo].[CONTACT]
SET [PHONE] = RTRIM([PHONE]);
GO


-- ============================================================
-- STEP 3 — sanity check: confirm the new column type, and that no
-- trailing whitespace remains.
-- ============================================================

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'CONTACT' AND COLUMN_NAME = 'PHONE';
GO

SELECT TOP 10 [AUTO_NUMBER], '[' + [PHONE] + ']' AS Bracketed
FROM [dbo].[CONTACT];
GO
