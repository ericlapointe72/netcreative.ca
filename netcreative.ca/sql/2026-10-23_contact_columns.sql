-- The contact form offers "Application boutique hors taxes" (31 characters) as a project type, and the PROJECT_TYPE column only holds 30:
-- choosing it made the INSERT fail ("String or binary data would be truncated") and the message was never stored.
-- The three columns that receive the drop-down lists are widened (the longest English text is 28 characters, the longest French 31).
-- One batch, without GO or USE: it runs as is in the site's "SQL commands" page (simulation first, then for real).
ALTER TABLE CONTACT ALTER COLUMN PROJECT_TYPE varchar(60) NULL;
ALTER TABLE CONTACT ALTER COLUMN PROJECT_BUDGET varchar(60) NULL;
ALTER TABLE CONTACT ALTER COLUMN PROJECT_DEADLINE varchar(60) NULL;
