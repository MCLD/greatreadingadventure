CREATE TABLE [dbo].[PatronRewardCodes] (
    [PRCID]       INT           IDENTITY (1, 1) NOT NULL,
    [PID]         INT           NULL,
    [BadgeID]     INT           NULL,
    [ProgID]      INT           NULL,
    [RewardCode]  VARCHAR (100) NULL,
    [LastModDate] DATETIME      CONSTRAINT [DF_PatronRewardCodes_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser] VARCHAR (50)  CONSTRAINT [DF_PatronRewardCodes_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]   DATETIME      CONSTRAINT [DF_PatronRewardCodes_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]   VARCHAR (50)  CONSTRAINT [DF_PatronRewardCodes_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_PatronRewardCodes] PRIMARY KEY CLUSTERED ([PRCID] ASC)
);

