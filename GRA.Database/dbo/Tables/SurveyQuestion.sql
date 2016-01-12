CREATE TABLE [dbo].[SurveyQuestion] (
    [QID]              INT           IDENTITY (1, 1) NOT NULL,
    [SID]              INT           NULL,
    [QNumber]          INT           NULL,
    [QType]            INT           NULL,
    [QName]            NVARCHAR(255) NULL,
    [QText]            TEXT          NULL,
    [DisplayControl]   INT           NULL,
    [DisplayDirection] INT           NULL,
    [IsRequired]       BIT           NULL,
    [FldInt1]          INT           NULL,
    [FldInt2]          INT           NULL,
    [FldInt3]          INT           NULL,
    [FldBit1]          BIT           NULL,
    [FldBit2]          BIT           NULL,
    [FldBit3]          BIT           NULL,
    [FldText1]         TEXT          NULL,
    [FldText2]         TEXT          NULL,
    [FldText3]         TEXT          NULL,
    CONSTRAINT [PK_SurveyQuestion] PRIMARY KEY CLUSTERED ([QID] ASC)
);

