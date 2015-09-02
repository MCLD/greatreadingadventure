CREATE TABLE [dbo].[SQChoices] (
    [SQCID]                 INT          IDENTITY (1, 1) NOT NULL,
    [QID]                   INT          NULL,
    [ChoiceOrder]           INT          NULL,
    [ChoiceText]            VARCHAR (50) NULL,
    [Score]                 INT          NULL,
    [JumpToQuestion]        INT          NULL,
    [AskClarification]      BIT          NULL,
    [ClarificationRequired] BIT          NULL,
    [FldInt1]               INT          NULL,
    [FldInt2]               INT          NULL,
    [FldInt3]               INT          NULL,
    [FldBit1]               BIT          NULL,
    [FldBit2]               BIT          NULL,
    [FldBit3]               BIT          NULL,
    [FldText1]              TEXT         NULL,
    [FldText2]              TEXT         NULL,
    [FldText3]              TEXT         NULL,
    CONSTRAINT [PK_SQChoices] PRIMARY KEY CLUSTERED ([SQCID] ASC)
);

