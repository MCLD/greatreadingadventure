CREATE TABLE [dbo].[SRPPermissionsMaster] (
    [PermissionID]   INT            NOT NULL,
    [PermissionName] NVARCHAR(255)   NULL,
    [PermissionDesc] NVARCHAR(MAX) NULL,
    [MODID]          INT            NULL,
    CONSTRAINT [PK_SRPPermissionsMaster] PRIMARY KEY CLUSTERED ([PermissionID] ASC)
);

