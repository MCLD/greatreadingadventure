CREATE TABLE [dbo].[MGMatchingGameTiles] (
    [MAGTID]         INT          IDENTITY (1, 1) NOT NULL,
    [MAGID]          INT          NOT NULL,
    [MGID]           INT          NULL,
    [Tile1UseMedium] BIT          NULL,
    [Tile1UseHard]   BIT          NULL,
    [Tile2UseMedium] BIT          NULL,
    [Tile2UseHard]   BIT          NULL,
    [Tile3UseMedium] BIT          NULL,
    [Tile3UseHard]   BIT          NULL,
    [LastModDate]    DATETIME     CONSTRAINT [DF_MGMatchingGameTiles_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]    VARCHAR (50) CONSTRAINT [DF_MGMatchingGameTiles_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]      DATETIME     CONSTRAINT [DF_MGMatchingGameTiles_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]      VARCHAR (50) CONSTRAINT [DF_MGMatchingGameTiles_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGMatchingGameTiles] PRIMARY KEY CLUSTERED ([MAGTID] ASC),
    CONSTRAINT [FK_MGMatchingGameTiles_MGMatchingGame] FOREIGN KEY ([MAGID]) REFERENCES [dbo].[MGMatchingGame] ([MAGID])
);

