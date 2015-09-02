CREATE TABLE [dbo].[PrizeDrawingWinners] (
    [PDWID]             INT          IDENTITY (1, 1) NOT NULL,
    [PDID]              INT          NULL,
    [PatronID]          INT          NULL,
    [NotificationID]    INT          NULL,
    [PrizePickedUpFlag] BIT          NULL,
    [LastModDate]       DATETIME     CONSTRAINT [DF_PrizeDrawingWinners_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]       VARCHAR (50) CONSTRAINT [DF_PrizeDrawingWinners_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]         DATETIME     CONSTRAINT [DF_PrizeDrawingWinners_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]         VARCHAR (50) CONSTRAINT [DF_PrizeDrawingWinners_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_PrizeDrawingWinners] PRIMARY KEY CLUSTERED ([PDWID] ASC)
);

