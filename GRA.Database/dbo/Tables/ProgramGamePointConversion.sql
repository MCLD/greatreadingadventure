CREATE TABLE [dbo].[ProgramGamePointConversion] (
    [PGCID]          INT          IDENTITY (1, 1) NOT NULL,
    [PGID]           INT          NULL,
    [ActivityTypeId] INT          NULL,
    [ActivityCount]  INT          NULL,
    [PointCount]     INT          NULL,
    [LastModDate]    DATETIME     CONSTRAINT [DF_ProgramGamePointConversion_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]    VARCHAR (50) CONSTRAINT [DF_ProgramGamePointConversion_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]      DATETIME     CONSTRAINT [DF_ProgramGamePointConversion_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]      VARCHAR (50) CONSTRAINT [DF_ProgramGamePointConversion_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_ProgramGamePointConversion] PRIMARY KEY CLUSTERED ([PGCID] ASC),
    CONSTRAINT [FK_ProgramGamePointConversion_Programs] FOREIGN KEY ([PGID]) REFERENCES [dbo].[Programs] ([PID])
);

