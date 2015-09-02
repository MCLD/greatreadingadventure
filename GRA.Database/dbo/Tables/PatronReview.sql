CREATE TABLE [dbo].[PatronReview] (
    [PRID]         INT           IDENTITY (1, 1) NOT NULL,
    [PID]          INT           NULL,
    [PRLID]        INT           NULL,
    [Author]       VARCHAR (50)  NULL,
    [Title]        VARCHAR (150) NULL,
    [Review]       TEXT          NULL,
    [isApproved]   BIT           NULL,
    [ReviewDate]   DATETIME      NULL,
    [ApprovalDate] DATETIME      NULL,
    [ApprovedBy]   VARCHAR (50)  NULL,
    CONSTRAINT [PK_PatronReview] PRIMARY KEY CLUSTERED ([PRID] ASC)
);

