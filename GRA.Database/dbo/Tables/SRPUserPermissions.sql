CREATE TABLE [dbo].[SRPUserPermissions] (
    [UID]          INT          NOT NULL,
    [PermissionID] INT          NOT NULL,
    [AddedDate]    DATETIME     CONSTRAINT [DF_SRPUserPermissions_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]    VARCHAR (50) CONSTRAINT [DF_SRPUserPermissions_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_SRPUserPermissions] PRIMARY KEY CLUSTERED ([UID] ASC, [PermissionID] ASC),
    CONSTRAINT [FK_UserPermissions_SRPUser] FOREIGN KEY ([UID]) REFERENCES [dbo].[SRPUser] ([UID])
);

