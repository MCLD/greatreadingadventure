CREATE TABLE [dbo].[SRPGroups] (
    [GID]              INT           IDENTITY (1000, 1) NOT NULL,
    [GroupName]        NVARCHAR(255)  NULL,
    [GroupDescription] VARCHAR (255) NULL,
    [LastModDate]      DATETIME      CONSTRAINT [DF_SRPGroups_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]      VARCHAR (50)  CONSTRAINT [DF_SRPGroups_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]        DATETIME      CONSTRAINT [DF_SRPGroups_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]        VARCHAR (50)  CONSTRAINT [DF_SRPGroups_AddedUser] DEFAULT ('N/A') NULL,
    [TenID]            INT           NULL,
    [FldInt1]          INT           NULL,
    [FldInt2]          INT           NULL,
    [FldInt3]          INT           NULL,
    [FldBit1]          BIT           NULL,
    [FldBit2]          BIT           NULL,
    [FldBit3]          BIT           NULL,
    [FldText1]         TEXT          NULL,
    [FldText2]         TEXT          NULL,
    [FldText3]         TEXT          NULL,
    CONSTRAINT [PK_SRPGroups] PRIMARY KEY CLUSTERED ([GID] ASC)
);

