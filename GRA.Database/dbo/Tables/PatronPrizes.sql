CREATE TABLE [dbo].[PatronPrizes] (
    [PPID]         INT          IDENTITY (1, 1) NOT NULL,
    [PID]          INT          NULL,
    [PrizeSource]  INT          NULL,
    [BadgeID]      INT          NULL,
    [DrawingID]    INT          NULL,
    [PrizeName]    VARCHAR (50) NULL,
    [RedeemedFlag] BIT          NULL,
    [LastModDate]  DATETIME     CONSTRAINT [DF_PatronPrizes_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]  VARCHAR (50) CONSTRAINT [DF_PatronPrizes_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]    DATETIME     CONSTRAINT [DF_PatronPrizes_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]    VARCHAR (50) CONSTRAINT [DF_PatronPrizes_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_PatronPrizes] PRIMARY KEY CLUSTERED ([PPID] ASC)
);

