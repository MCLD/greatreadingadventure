CREATE TABLE [dbo].[PatronReview] (
    [PRID]         INT           IDENTITY (1, 1) NOT NULL,
    [PID]          INT           NULL,
    [PRLID]        INT           NULL,
    [Author]       NVARCHAR(255)  NULL,
    [Title]        NVARCHAR(255) NULL,
    [Review]       TEXT          NULL,
    [isApproved]   BIT           NULL,
    [ReviewDate]   DATETIME      NULL,
    [ApprovalDate] DATETIME      NULL,
    [ApprovedBy]   VARCHAR (50)  NULL,
    CONSTRAINT [PK_PatronReview] PRIMARY KEY CLUSTERED ([PRID] ASC)
);

