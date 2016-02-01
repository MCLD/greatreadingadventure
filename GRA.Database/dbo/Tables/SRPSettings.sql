CREATE TABLE [dbo].[SRPSettings] (
    [SID]          INT            IDENTITY (1000, 1) NOT NULL,
    [Name]         NVARCHAR(255)   NULL,
    [Value]        TEXT           NULL,
    [StorageType]  VARCHAR (50)   CONSTRAINT [DF_SRPSettings_StorageType] DEFAULT ('Text') NULL,
    [EditType]     VARCHAR (50)   CONSTRAINT [DF_SRPSettings_EditType] DEFAULT ('TextBox') NULL,
    [ModID]        INT            NULL,
    [Label]        VARCHAR (50)   NULL,
    [Description]  VARCHAR (500)  NULL,
    [ValueList]    NVARCHAR(MAX) NULL,
    [DefaultValue] TEXT           NULL,
    [TenID]        INT            NULL,
    [FldInt1]      INT            NULL,
    [FldInt2]      INT            NULL,
    [FldInt3]      INT            NULL,
    [FldBit1]      BIT            NULL,
    [FldBit2]      BIT            NULL,
    [FldBit3]      BIT            NULL,
    [FldText1]     TEXT           NULL,
    [FldText2]     TEXT           NULL,
    [FldText3]     TEXT           NULL,
    CONSTRAINT [PK_SRPSettings] PRIMARY KEY CLUSTERED ([SID] ASC)
);

