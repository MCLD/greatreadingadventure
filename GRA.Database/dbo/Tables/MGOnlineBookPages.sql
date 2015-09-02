CREATE TABLE [dbo].[MGOnlineBookPages] (
    [OBPGID]      INT           IDENTITY (1, 1) NOT NULL,
    [OBID]        INT           NULL,
    [MGID]        INT           NULL,
    [PageNumber]  INT           NULL,
    [TextEasy]    TEXT          NULL,
    [TextMedium]  TEXT          NULL,
    [TextHard]    TEXT          NULL,
    [AudioEasy]   VARCHAR (150) NULL,
    [AudioMedium] VARCHAR (150) NULL,
    [AudioHard]   VARCHAR (150) NULL,
    [LastModDate] DATETIME      CONSTRAINT [DF_MGOnlineBookPages_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser] VARCHAR (50)  CONSTRAINT [DF_MGOnlineBookPages_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]   DATETIME      CONSTRAINT [DF_MGOnlineBookPages_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]   VARCHAR (50)  CONSTRAINT [DF_MGOnlineBookPages_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGOnlineBookPages] PRIMARY KEY CLUSTERED ([OBPGID] ASC),
    CONSTRAINT [FK_MGOnlineBookPages_MGOnlineBook] FOREIGN KEY ([OBID]) REFERENCES [dbo].[MGOnlineBook] ([OBID])
);

