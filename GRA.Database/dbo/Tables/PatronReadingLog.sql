CREATE TABLE [dbo].[PatronReadingLog] (
    [PRLID]            INT           IDENTITY (1, 1) NOT NULL,
    [PID]              INT           NULL,
    [ReadingType]      INT           NULL,
    [ReadingTypeLabel] VARCHAR (50)  NULL,
    [ReadingAmount]    INT           NULL,
    [ReadingPoints]    INT           NULL,
    [LoggingDate]      VARCHAR (50)  NULL,
    [Author]           NVARCHAR (255)  NULL,
    [Title]            NVARCHAR (255) NULL,
    [HasReview]        BIT           NULL,
    [ReviewID]         INT           NULL,
    [LoggedAt] DATETIME NULL, 
    CONSTRAINT [PK_PatronReadingLog] PRIMARY KEY CLUSTERED ([PRLID] ASC)
);

