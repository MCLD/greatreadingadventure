CREATE TABLE [dbo].[Code] (
    [CID]         INT          IDENTITY (1, 1) NOT NULL,
    [CTID]        INT          NULL,
    [Code]        NVARCHAR(255) NULL,
    [Description] NVARCHAR(255) NULL,
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
    CONSTRAINT [PK_Code] PRIMARY KEY CLUSTERED ([CID] ASC),
    CONSTRAINT [FK_Code_CodeType] FOREIGN KEY ([CTID]) REFERENCES [dbo].[CodeType] ([CTID])
);

