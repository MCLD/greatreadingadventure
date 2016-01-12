CREATE TABLE [dbo].[SentEmailLog] (
    [EID]          INT           IDENTITY (1, 1) NOT NULL,
    [SentDateTime] DATETIME      CONSTRAINT [DF_SentEmailLog_SentDateTime] DEFAULT (getdate()) NULL,
    [SentFrom]     NVARCHAR(255) NULL,
    [SentTo]       NVARCHAR(255) NULL,
    [Subject]      NVARCHAR(255) NULL,
    [Body]         TEXT          NULL,
    CONSTRAINT [PK_SentEmailLog] PRIMARY KEY CLUSTERED ([EID] ASC)
);

