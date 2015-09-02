CREATE TABLE [dbo].[SRPPermissionsMaster] (
    [PermissionID]   INT            NOT NULL,
    [PermissionName] VARCHAR (50)   NULL,
    [PermissionDesc] VARCHAR (2000) NULL,
    [MODID]          INT            NULL,
    CONSTRAINT [PK_SRPPermissionsMaster] PRIMARY KEY CLUSTERED ([PermissionID] ASC)
);

