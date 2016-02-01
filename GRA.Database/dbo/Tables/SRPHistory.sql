CREATE TABLE [dbo].[SRPHistory]
(
	[Id] INT NOT NULL PRIMARY KEY, 
	[When] DATETIME NOT NULL,
	[Who] NVARCHAR(255) NOT NULL,
    [Event] NVARCHAR(255) NOT NULL, 
    [VersionMajor] INT NOT NULL, 
    [VersionMinor] INT NOT NULL, 
    [VersionPatch] INT NOT NULL, 
    [Description] NVARCHAR(MAX) NULL
)
