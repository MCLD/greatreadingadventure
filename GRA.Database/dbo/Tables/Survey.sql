CREATE TABLE [dbo].[Survey] (
    [SID]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR(255)  NULL,
    [LongName]    NVARCHAR(255) NULL,
    [Description] TEXT          NULL,
    [Preamble]    TEXT          NULL,
    [Status]      INT           NULL,
    [TakenCount]  INT           NULL,
    [PatronCount] INT           NULL,
    [CanBeScored] BIT           NULL,
    [TenID]       INT           NULL,
    [FldInt1]     INT           NULL,
    [FldInt2]     INT           NULL,
    [FldInt3]     INT           NULL,
    [FldBit1]     BIT           NULL,
    [FldBit2]     BIT           NULL,
    [FldBit3]     BIT           NULL,
    [FldText1]    TEXT          NULL,
    [FldText2]    TEXT          NULL,
    [FldText3]    TEXT          NULL,
    CONSTRAINT [PK_Survey] PRIMARY KEY CLUSTERED ([SID] ASC)
);

