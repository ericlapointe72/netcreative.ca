/*
    USERS table - two-factor sign-in for the admin
    ======================================================================
    The admin can ask for a 6-digit code from an authenticator app (Google
    Authenticator, Microsoft Authenticator, Authy...) after the password.
    Four columns keep what that needs, per user:

        TotpSecret    nvarchar(400)  the secret of the app, ENCRYPTED with the
                                     site's machine key (never readable here)
        TotpEnabled   bit            1 once the user has turned it on
        TotpLastStep  bigint         the last 30-second step accepted, so a
                                     code cannot be used twice
        TotpRecovery  nvarchar(600)  hashes of the recovery codes (one use each)

    The site works without them: until the columns exist, nobody is asked for
    a code. So the code can be deployed first and this script run afterwards.
    Nothing is turned on for anybody by this script: each user turns it on
    from the admin menu "Double authentification".

    HOW TO RUN IT - this file works in SSMS AND in the admin page
    "Commandes SQL": it has no GO and no USE, so each step below is ONE
    block of text that can be pasted as it is. (In SSMS, first check that the
    connection is on the NETCREATIVE database.) Take a backup of the USERS
    table before Step 1. Run one step at a time, in order; each step can be
    run again without harm.
*/


-- ============================================================
-- STEP 1 - add the four columns (copy from here to the line
-- "END OF STEP 1"). Existing users get TotpEnabled = 0 (two-factor
-- off), the other three stay NULL.
-- ============================================================

IF COL_LENGTH('dbo.USERS', 'TotpSecret') IS NULL
    ALTER TABLE [dbo].[USERS] ADD [TotpSecret] nvarchar(400) NULL;

IF COL_LENGTH('dbo.USERS', 'TotpEnabled') IS NULL
    ALTER TABLE [dbo].[USERS] ADD [TotpEnabled] bit NOT NULL CONSTRAINT [DF_USERS_TotpEnabled] DEFAULT (0);

IF COL_LENGTH('dbo.USERS', 'TotpLastStep') IS NULL
    ALTER TABLE [dbo].[USERS] ADD [TotpLastStep] bigint NULL;

IF COL_LENGTH('dbo.USERS', 'TotpRecovery') IS NULL
    ALTER TABLE [dbo].[USERS] ADD [TotpRecovery] nvarchar(600) NULL;

-- END OF STEP 1


-- ============================================================
-- STEP 2 - check (run it by itself, after Step 1): the list must
-- show the four Totp columns.
-- ============================================================

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'USERS'
ORDER BY ORDINAL_POSITION;


-- ============================================================
-- LATER, IF NEEDED - a user lost their phone AND their recovery
-- codes. This puts their account back to "two-factor off": they
-- sign in with their password, then turn it on again. Change the
-- name, run it by itself.
-- ============================================================

/*
UPDATE [dbo].[USERS]
SET [TotpSecret] = NULL, [TotpEnabled] = 0, [TotpLastStep] = NULL, [TotpRecovery] = NULL
WHERE [USER_NAME] = 'the-user-name';
*/
