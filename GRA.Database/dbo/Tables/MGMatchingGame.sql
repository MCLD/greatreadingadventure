CREATE TABLE [dbo].[MGMatchingGame] (
    [MAGID]                   INT          IDENTITY (1, 1) NOT NULL,
    [MGID]                    INT          NULL,
    [CorrectRoundsToWinCount] INT          NULL,
    [EasyGameSize]            INT          CONSTRAINT [DF_MGMatchingGame_EasyGameSize] DEFAULT ((2)) NULL,
    [MediumGameSize]          INT          CONSTRAINT [DF_MGMatchingGame_MediumGameSize] DEFAULT ((4)) NULL,
    [HardGameSize]            INT          CONSTRAINT [DF_MGMatchingGame_HardGameSize] DEFAULT ((6)) NULL,
    [EnableMediumDifficulty]  BIT          NULL,
    [EnableHardDifficulty]    BIT          NULL,
    [LastModDate]             DATETIME     CONSTRAINT [DF_MGMatchingGame_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]             VARCHAR (50) CONSTRAINT [DF_MGMatchingGame_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]               DATETIME     CONSTRAINT [DF_MGMatchingGame_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]               VARCHAR (50) CONSTRAINT [DF_MGMatchingGame_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGMatchingGame] PRIMARY KEY CLUSTERED ([MAGID] ASC),
    CONSTRAINT [FK_MGMatchingGame_Minigame] FOREIGN KEY ([MGID]) REFERENCES [dbo].[Minigame] ([MGID])
);

