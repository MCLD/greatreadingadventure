CREATE TABLE [dbo].[MGWordMatchItems] (
    [WMIID]       INT           IDENTITY (1, 1) NOT NULL,
    [WMID]        INT           NOT NULL,
    [MGID]        INT           NULL,
    [ItemImage]   VARCHAR (150) NULL,
    [EasyLabel]   VARCHAR (150) NULL,
    [MediumLabel] VARCHAR (150) NULL,
    [HardLabel]   VARCHAR (150) NULL,
    [AudioEasy]   VARCHAR (150) NULL,
    [AudioMedium] VARCHAR (150) NULL,
    [AudioHard]   VARCHAR (150) NULL,
    [LastModDate] DATETIME      CONSTRAINT [DF_MGWordMatchItems_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser] VARCHAR (50)  CONSTRAINT [DF_MGWordMatchItems_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]   DATETIME      CONSTRAINT [DF_MGWordMatchItems_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]   VARCHAR (50)  CONSTRAINT [DF_MGWordMatchItems_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGWordMatchItems] PRIMARY KEY CLUSTERED ([WMIID] ASC),
    CONSTRAINT [FK_MGWordMatchItems_MGWordMatch] FOREIGN KEY ([WMID]) REFERENCES [dbo].[MGWordMatch] ([WMID])
);

