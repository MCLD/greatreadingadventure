CREATE TABLE [dbo].[ProgramCodes] (
    [PCID]        INT              IDENTITY (1, 1) NOT NULL,
    [PID]         INT              NULL,
    [CodeNumber]  INT              NULL,
    [CodeValue]   UNIQUEIDENTIFIER NULL,
    [isUsed]      BIT              CONSTRAINT [DF_ProgramCodes_isUsed] DEFAULT ((0)) NULL,
    [DateCreated] DATETIME         CONSTRAINT [DF_ProgramCodes_DateCreated] DEFAULT (getdate()) NULL,
    [DateUsed]    DATETIME         NULL,
    [PatronId]    INT              NULL,
    [ShortCode]   VARCHAR (20)     NULL,
    CONSTRAINT [PK_ProgramCodes] PRIMARY KEY CLUSTERED ([PCID] ASC),
    CONSTRAINT [FK_ProgramCodes_Programs] FOREIGN KEY ([PID]) REFERENCES [dbo].[Programs] ([PID])
);

