CREATE TABLE [dbo].[MGWordMatch] (
    [WMID]                    INT          IDENTITY (1, 1) NOT NULL,
    [MGID]                    INT          NULL,
    [CorrectRoundsToWinCount] INT          NULL,
    [NumOptionsToChooseFrom]  INT          NULL,
    [EnableMediumDifficulty]  BIT          NULL,
    [EnableHardDifficulty]    BIT          NULL,
    [LastModDate]             DATETIME     CONSTRAINT [DF_MGWordMatch_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]             VARCHAR (50) CONSTRAINT [DF_MGWordMatch_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]               DATETIME     CONSTRAINT [DF_MGWordMatch_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]               VARCHAR (50) CONSTRAINT [DF_MGWordMatch_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_WGMixAndMatch] PRIMARY KEY CLUSTERED ([WMID] ASC),
    CONSTRAINT [FK_MGWordMatch_Minigame] FOREIGN KEY ([MGID]) REFERENCES [dbo].[Minigame] ([MGID])
);

