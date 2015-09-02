CREATE TABLE [dbo].[SQMatrixLines] (
    [SQMLID]    INT           IDENTITY (1, 1) NOT NULL,
    [QID]       INT           NULL,
    [LineOrder] INT           NULL,
    [LineText]  VARCHAR (500) NULL,
    [FldInt1]   INT           NULL,
    [FldInt2]   INT           NULL,
    [FldInt3]   INT           NULL,
    [FldBit1]   BIT           NULL,
    [FldBit2]   BIT           NULL,
    [FldBit3]   BIT           NULL,
    [FldText1]  TEXT          NULL,
    [FldText2]  TEXT          NULL,
    [FldText3]  TEXT          NULL,
    CONSTRAINT [PK_SQMatrixLines] PRIMARY KEY CLUSTERED ([SQMLID] ASC)
);

