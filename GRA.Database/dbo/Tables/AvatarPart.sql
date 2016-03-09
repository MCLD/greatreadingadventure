CREATE TABLE [dbo].[AvatarPart] (
    [APID]        INT          IDENTITY (1, 1) NOT NULL,
	[ComponentID] INT NULL,
    [BadgeID]     INT   NULL,
    [Name]        VARCHAR (50) NULL,
    [LastModDate] DATETIME     CONSTRAINT [DF_AvatarPart_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser] VARCHAR (50) CONSTRAINT [DF_AvatarPart_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]   DATETIME     CONSTRAINT [DF_AvatarPart_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]   VARCHAR (50) CONSTRAINT [DF_AvatarPart_AddedUser] DEFAULT ('N/A') NULL,
    [TenID]       INT          NULL,
    CONSTRAINT [PK_AvatarPart] PRIMARY KEY CLUSTERED ([APID] ASC)
);

