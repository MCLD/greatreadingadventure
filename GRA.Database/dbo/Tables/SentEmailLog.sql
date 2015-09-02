CREATE TABLE [dbo].[SentEmailLog] (
    [EID]          INT           IDENTITY (1, 1) NOT NULL,
    [SentDateTime] DATETIME      CONSTRAINT [DF_SentEmailLog_SentDateTime] DEFAULT (getdate()) NULL,
    [SentFrom]     VARCHAR (150) NULL,
    [SentTo]       VARCHAR (150) NULL,
    [Subject]      VARCHAR (150) NULL,
    [Body]         TEXT          NULL,
    CONSTRAINT [PK_SentEmailLog] PRIMARY KEY CLUSTERED ([EID] ASC)
);

