CREATE TABLE [dbo].[MGMixAndMatchItems] (
    [MMIID]       INT           IDENTITY (1, 1) NOT NULL,
    [MMID]        INT           NOT NULL,
    [MGID]        INT           NULL,
    [ItemImage]   VARCHAR (150) NULL,
    [EasyLabel]   VARCHAR (150) NULL,
    [MediumLabel] VARCHAR (150) NULL,
    [HardLabel]   VARCHAR (150) NULL,
    [AudioEasy]   VARCHAR (150) NULL,
    [AudioMedium] VARCHAR (150) NULL,
    [AudioHard]   VARCHAR (150) NULL,
    [LastModDate] DATETIME      CONSTRAINT [DF_MGMixAndMatchItems_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser] VARCHAR (50)  CONSTRAINT [DF_MGMixAndMatchItems_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]   DATETIME      CONSTRAINT [DF_MGMixAndMatchItems_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]   VARCHAR (50)  CONSTRAINT [DF_MGMixAndMatchItems_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGMixAndMatchItems] PRIMARY KEY CLUSTERED ([MMIID] ASC),
    CONSTRAINT [FK_MGMixAndMatchItems_MGMixAndMatch] FOREIGN KEY ([MMID]) REFERENCES [dbo].[MGMixAndMatch] ([MMID])
);

