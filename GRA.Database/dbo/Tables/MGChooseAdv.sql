CREATE TABLE [dbo].[MGChooseAdv] (
    [CAID]                   INT          IDENTITY (1, 1) NOT NULL,
    [MGID]                   INT          NULL,
    [EnableMediumDifficulty] BIT          NULL,
    [EnableHardDifficulty]   BIT          NULL,
    [LastModDate]            DATETIME     CONSTRAINT [DF_MGChooseAdv_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]            VARCHAR (50) CONSTRAINT [DF_MGChooseAdv_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]              DATETIME     CONSTRAINT [DF_MGChooseAdv_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]              VARCHAR (50) CONSTRAINT [DF_MGChooseAdv_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGChooseAdv] PRIMARY KEY CLUSTERED ([CAID] ASC),
    CONSTRAINT [FK_MGChooseAdv_Minigame] FOREIGN KEY ([MGID]) REFERENCES [dbo].[Minigame] ([MGID])
);

