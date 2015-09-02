CREATE TABLE [dbo].[MGHiddenPicBk] (
    [HPBID]       INT          IDENTITY (1, 1) NOT NULL,
    [HPID]        INT          NOT NULL,
    [MGID]        INT          NULL,
    [LastModDate] DATETIME     CONSTRAINT [DF_MGHiddenPicBk_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser] VARCHAR (50) CONSTRAINT [DF_MGHiddenPicBk_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]   DATETIME     CONSTRAINT [DF_MGHiddenPicBk_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]   VARCHAR (50) CONSTRAINT [DF_MGHiddenPicBk_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGHiddenPicBk] PRIMARY KEY CLUSTERED ([HPBID] ASC),
    CONSTRAINT [FK_MGHiddenPicBk_MGHiddenPic1] FOREIGN KEY ([HPID]) REFERENCES [dbo].[MGHiddenPic] ([HPID])
);

