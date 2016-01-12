CREATE TABLE [dbo].[CodeType] (
    [CTID]         INT          IDENTITY (1, 1) NOT NULL,
    [isSystem]     BIT          CONSTRAINT [DF_CodeType_isSystem] DEFAULT ((0)) NULL,
    [CodeTypeName] NVARCHAR(255) NULL,
    [Description]  TEXT         NULL,
    [TenID]        INT          NULL,
    [FldInt1]      INT          NULL,
    [FldInt2]      INT          NULL,
    [FldInt3]      INT          NULL,
    [FldBit1]      BIT          NULL,
    [FldBit2]      BIT          NULL,
    [FldBit3]      BIT          NULL,
    [FldText1]     TEXT         NULL,
    [FldText2]     TEXT         NULL,
    [FldText3]     TEXT         NULL,
    CONSTRAINT [PK_CodeType] PRIMARY KEY CLUSTERED ([CTID] ASC)
);

