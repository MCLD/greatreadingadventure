CREATE TABLE [dbo].[PatronBadges] (
    [PBID]       INT      IDENTITY (1, 1) NOT NULL,
    [PID]        INT      NULL,
    [BadgeID]    INT      NULL,
    [DateEarned] DATETIME NULL,
    CONSTRAINT [PK_PatronBadges] PRIMARY KEY CLUSTERED ([PBID] ASC)
);

