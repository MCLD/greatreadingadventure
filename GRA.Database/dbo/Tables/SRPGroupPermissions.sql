CREATE TABLE [dbo].[SRPGroupPermissions] (
    [GID]          INT          NOT NULL,
    [PermissionID] INT          NOT NULL,
    [AddedDate]    DATETIME     CONSTRAINT [DF_SRPGroupPermissions_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]    VARCHAR (50) CONSTRAINT [DF_SRPGroupPermissions_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_GroupPermissions] PRIMARY KEY CLUSTERED ([GID] ASC, [PermissionID] ASC),
    CONSTRAINT [FK_GroupPermissions_SRPGroups] FOREIGN KEY ([GID]) REFERENCES [dbo].[SRPGroups] ([GID])
);

