CREATE TABLE [dbo].[PatronBookLists] (
    [PBLBID]      INT      IDENTITY (1, 1) NOT NULL,
    [PID]         INT      NULL,
    [BLBID]       INT      NOT NULL,
    [BLID]        INT      NULL,
    [HasReadFlag] BIT      NULL,
    [LastModDate] DATETIME CONSTRAINT [DF_PatronBookLists_LastModDate] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_PatronBookLists] PRIMARY KEY CLUSTERED ([PBLBID] ASC)
);

