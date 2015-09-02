CREATE TABLE [dbo].[MGMixAndMatch] (
    [MMID]                    INT          IDENTITY (1, 1) NOT NULL,
    [MGID]                    INT          NULL,
    [CorrectRoundsToWinCount] INT          NULL,
    [EnableMediumDifficulty]  BIT          NULL,
    [EnableHardDifficulty]    BIT          NULL,
    [LastModDate]             DATETIME     CONSTRAINT [DF_MGMixAndMatch_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]             VARCHAR (50) CONSTRAINT [DF_MGMixAndMatch_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]               DATETIME     CONSTRAINT [DF_MGMixAndMatch_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]               VARCHAR (50) CONSTRAINT [DF_MGMixAndMatch_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGMixAndMatch] PRIMARY KEY CLUSTERED ([MMID] ASC),
    CONSTRAINT [FK_MGMixAndMatch_Minigame] FOREIGN KEY ([MGID]) REFERENCES [dbo].[Minigame] ([MGID])
);

