CREATE TABLE [dbo].[ProgramGameLevel] (
    [PGLID]             INT          IDENTITY (1, 1) NOT NULL,
    [PGID]              INT          NULL,
    [LevelNumber]       INT          NULL,
    [LocationX]         INT          NULL,
    [LocationY]         INT          NULL,
    [LocationXBonus]    INT          NULL,
    [LocationYBonus]    INT          NULL,
    [PointNumber]       INT          NULL,
    [Minigame1ID]       INT          NULL,
    [Minigame2ID]       INT          NULL,
    [AwardBadgeID]      INT          NULL,
    [Minigame1IDBonus]  INT          NULL,
    [Minigame2IDBonus]  INT          NULL,
    [AwardBadgeIDBonus] INT          NULL,
    [LastModDate]       DATETIME     CONSTRAINT [DF_ProgramGameLevel_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]       VARCHAR (50) CONSTRAINT [DF_ProgramGameLevel_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]         DATETIME     CONSTRAINT [DF_ProgramGameLevel_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]         VARCHAR (50) CONSTRAINT [DF_ProgramGameLevel_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_ProgramGameLevel] PRIMARY KEY CLUSTERED ([PGLID] ASC),
    CONSTRAINT [FK_ProgramGameLevel_ProgramGame] FOREIGN KEY ([PGID]) REFERENCES [dbo].[ProgramGame] ([PGID])
);

