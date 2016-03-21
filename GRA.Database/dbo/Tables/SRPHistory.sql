CREATE TABLE [dbo].[SRPHistory] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [When]         DATETIME       NOT NULL,
    [Who]          NVARCHAR (255) NOT NULL,
    [Event]        NVARCHAR (255) NOT NULL,
    [VersionMajor] INT            NOT NULL,
    [VersionMinor] INT            NOT NULL,
    [VersionPatch] INT            NOT NULL,
    [Description]  NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


