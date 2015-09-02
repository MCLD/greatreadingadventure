CREATE TABLE [dbo].[MGHiddenPic] (
    [HPID]                   INT          IDENTITY (1, 1) NOT NULL,
    [MGID]                   INT          NULL,
    [EnableMediumDifficulty] BIT          NULL,
    [EnableHardDifficulty]   BIT          NULL,
    [EasyDictionary]         TEXT         NULL,
    [MediumDictionary]       TEXT         NULL,
    [HardDictionary]         TEXT         NULL,
    [LastModDate]            DATETIME     CONSTRAINT [DF_MGHiddenPic_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]            VARCHAR (50) CONSTRAINT [DF_MGHiddenPic_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]              DATETIME     CONSTRAINT [DF_MGHiddenPic_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]              VARCHAR (50) CONSTRAINT [DF_MGHiddenPic_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGHiddenPic] PRIMARY KEY CLUSTERED ([HPID] ASC),
    CONSTRAINT [FK_MGHiddenPic_Minigame] FOREIGN KEY ([MGID]) REFERENCES [dbo].[Minigame] ([MGID])
);

