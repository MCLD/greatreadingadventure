CREATE TABLE [dbo].[PrizeDrawing] (
    [PDID]            INT           IDENTITY (1, 1) NOT NULL,
    [PrizeName]       VARCHAR (250) NULL,
    [TID]             INT           NULL,
    [DrawingDateTime] DATETIME      NULL,
    [NumWinners]      INT           NULL,
    [LastModDate]     DATETIME      CONSTRAINT [DF_PrizeDrawing_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]     VARCHAR (50)  CONSTRAINT [DF_PrizeDrawing_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]       DATETIME      CONSTRAINT [DF_PrizeDrawing_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]       VARCHAR (50)  CONSTRAINT [DF_PrizeDrawing_AddedUser] DEFAULT ('N/A') NULL,
    [TenID]           INT           NULL,
    [FldInt1]         INT           NULL,
    [FldInt2]         INT           NULL,
    [FldInt3]         INT           NULL,
    [FldBit1]         BIT           NULL,
    [FldBit2]         BIT           NULL,
    [FldBit3]         BIT           NULL,
    [FldText1]        TEXT          NULL,
    [FldText2]        TEXT          NULL,
    [FldText3]        TEXT          NULL,
    CONSTRAINT [PK_PrizeDrawing] PRIMARY KEY CLUSTERED ([PDID] ASC)
);

