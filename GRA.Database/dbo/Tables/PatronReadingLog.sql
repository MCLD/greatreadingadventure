CREATE TABLE [dbo].[PatronReadingLog] (
    [PRLID]            INT           IDENTITY (1, 1) NOT NULL,
    [PID]              INT           NULL,
    [ReadingType]      INT           NULL,
    [ReadingTypeLabel] VARCHAR (50)  NULL,
    [ReadingAmount]    INT           NULL,
    [ReadingPoints]    INT           NULL,
    [LoggingDate]      VARCHAR (50)  NULL,
    [Author]           VARCHAR (50)  NULL,
    [Title]            VARCHAR (150) NULL,
    [HasReview]        BIT           NULL,
    [ReviewID]         INT           NULL,
    CONSTRAINT [PK_PatronReadingLog] PRIMARY KEY CLUSTERED ([PRLID] ASC)
);

