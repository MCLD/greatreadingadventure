CREATE TABLE [dbo].[Avatar] (
    [AID]         INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NULL,
    [Gender]      VARCHAR (1)  NULL,
    [LastModDate] DATETIME     CONSTRAINT [DF_Avatar_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser] VARCHAR (50) CONSTRAINT [DF_Avatar_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]   DATETIME     CONSTRAINT [DF_Avatar_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]   VARCHAR (50) CONSTRAINT [DF_Avatar_AddedUser] DEFAULT ('N/A') NULL,
    [TenID]       INT          NULL,
    [FldInt1]     INT          NULL,
    [FldInt2]     INT          NULL,
    [FldInt3]     INT          NULL,
    [FldBit1]     BIT          NULL,
    [FldBit2]     BIT          NULL,
    [FldBit3]     BIT          NULL,
    [FldText1]    TEXT         NULL,
    [FldText2]    TEXT         NULL,
    [FldText3]    TEXT         NULL,
    CONSTRAINT [PK_Avatar] PRIMARY KEY CLUSTERED ([AID] ASC)
);

