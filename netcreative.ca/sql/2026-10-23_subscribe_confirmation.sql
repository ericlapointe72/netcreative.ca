-- Double confirmation of the newsletter, like beaucedutyfree.com: an address only receives the newsletter once its owner clicked the link
-- of the confirmation e-mail. Three columns are added to SUBSCRIBE:
--   CONFIRM_TOKEN  the one-time key of the link (erased once used)
--   CONFIRM_SENT   when the confirmation e-mail left (UTC): the link is valid for 7 days, one e-mail per address per day
--   CONFIRMED      when the address was confirmed (UTC); empty = waiting for the click
-- The addresses already on the list keep receiving the newsletter: they are marked confirmed now.
-- The e-mail column is widened to hold any valid address (254 characters) when it is shorter.
-- One batch, without GO or USE: it runs as is in the site's "SQL commands" page (simulation first, then for real).
-- The UPDATE is inside EXEC because a batch is read as a whole before it runs: the new columns do not exist yet at that point.
ALTER TABLE SUBSCRIBE ADD CONFIRM_TOKEN char(32) NULL, CONFIRM_SENT datetime2(0) NULL, CONFIRMED datetime2(0) NULL;
EXEC('UPDATE SUBSCRIBE SET CONFIRMED = SYSUTCDATETIME() WHERE CONFIRMED IS NULL');
IF (SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SUBSCRIBE' AND COLUMN_NAME = 'EMAIL') < 254 EXEC('ALTER TABLE SUBSCRIBE ALTER COLUMN EMAIL varchar(254) NULL');
