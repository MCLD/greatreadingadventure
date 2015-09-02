CREATE TABLE [dbo].[MGCodeBreaker] (
    [CBID]                   INT           IDENTITY (1, 1) NOT NULL,
    [MGID]                   INT           NULL,
    [EasyString]             VARCHAR (250) NULL,
    [EnableMediumDifficulty] BIT           NULL,
    [EnableHardDifficulty]   BIT           NULL,
    [MediumString]           VARCHAR (250) NULL,
    [HardString]             VARCHAR (250) NULL,
    [LastModDate]            DATETIME      CONSTRAINT [DF_MGCodeBreaker_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]            VARCHAR (50)  CONSTRAINT [DF_MGCodeBreaker_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]              DATETIME      CONSTRAINT [DF_MGCodeBreaker_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]              VARCHAR (50)  CONSTRAINT [DF_MGCodeBreaker_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGCodeBreaker] PRIMARY KEY CLUSTERED ([CBID] ASC),
    CONSTRAINT [FK_MGCodeBreaker_Minigame] FOREIGN KEY ([MGID]) REFERENCES [dbo].[Minigame] ([MGID])
);

