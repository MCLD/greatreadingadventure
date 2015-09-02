CREATE TABLE [dbo].[GamePlayStats] (
    [GPSID]         INT          IDENTITY (1, 1) NOT NULL,
    [PID]           INT          NULL,
    [MGID]          INT          NULL,
    [MGType]        INT          NULL,
    [Started]       DATETIME     CONSTRAINT [DF_GamePlayStats_Started] DEFAULT (getdate()) NULL,
    [Difficulty]    VARCHAR (50) NULL,
    [CompletedPlay] BIT          CONSTRAINT [DF_GamePlayStats_CompletedPlay] DEFAULT ((0)) NULL,
    [Completed]     DATETIME     NULL,
    CONSTRAINT [PK_GamePlayStats] PRIMARY KEY CLUSTERED ([GPSID] ASC)
);

