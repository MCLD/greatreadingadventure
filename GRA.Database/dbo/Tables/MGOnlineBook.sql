CREATE TABLE [dbo].[MGOnlineBook] (
    [OBID]                   INT          IDENTITY (1, 1) NOT NULL,
    [MGID]                   INT          NULL,
    [EnableMediumDifficulty] BIT          NULL,
    [EnableHardDifficulty]   BIT          NULL,
    [LastModDate]            DATETIME     CONSTRAINT [DF_MGOnlineBook_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]            VARCHAR (50) CONSTRAINT [DF_MGOnlineBook_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]              DATETIME     CONSTRAINT [DF_MGOnlineBook_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]              VARCHAR (50) CONSTRAINT [DF_MGOnlineBook_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGOnlineBook] PRIMARY KEY CLUSTERED ([OBID] ASC),
    CONSTRAINT [FK_MGOnlineBook_Minigame] FOREIGN KEY ([MGID]) REFERENCES [dbo].[Minigame] ([MGID])
);

