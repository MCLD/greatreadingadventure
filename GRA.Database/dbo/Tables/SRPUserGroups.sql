CREATE TABLE [dbo].[SRPUserGroups] (
    [UID]       INT          NOT NULL,
    [GID]       INT          NOT NULL,
    [AddedDate] DATETIME     CONSTRAINT [DF_SRPUserGroups_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser] VARCHAR (50) CONSTRAINT [DF_SRPUserGroups_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_SRPUserGroups] PRIMARY KEY CLUSTERED ([UID] ASC, [GID] ASC),
    CONSTRAINT [FK_SRPUserGroups_SRPGroups] FOREIGN KEY ([GID]) REFERENCES [dbo].[SRPGroups] ([GID]),
    CONSTRAINT [FK_SRPUserGroups_SRPUser] FOREIGN KEY ([UID]) REFERENCES [dbo].[SRPUser] ([UID])
);

