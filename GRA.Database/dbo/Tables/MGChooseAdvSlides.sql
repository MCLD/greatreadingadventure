CREATE TABLE [dbo].[MGChooseAdvSlides] (
    [CASID]               INT          IDENTITY (1, 1) NOT NULL,
    [CAID]                INT          NOT NULL,
    [MGID]                INT          NULL,
    [Difficulty]          INT          CONSTRAINT [DF_MGChooseAdvSlides_Difficulty] DEFAULT ((1)) NULL,
    [StepNumber]          INT          CONSTRAINT [DF_MGChooseAdvSlides_StepNumber] DEFAULT ((-1)) NULL,
    [SlideText]           TEXT         NULL,
    [FirstImageGoToStep]  INT          NULL,
    [SecondImageGoToStep] INT          NULL,
    [LastModDate]         DATETIME     CONSTRAINT [DF_MGChooseAdvSlides_LastModDate] DEFAULT (getdate()) NULL,
    [LastModUser]         VARCHAR (50) CONSTRAINT [DF_MGChooseAdvSlides_LastModUser] DEFAULT ('N/A') NULL,
    [AddedDate]           DATETIME     CONSTRAINT [DF_MGChooseAdvSlides_AddedDate] DEFAULT (getdate()) NULL,
    [AddedUser]           VARCHAR (50) CONSTRAINT [DF_MGChooseAdvSlides_AddedUser] DEFAULT ('N/A') NULL,
    CONSTRAINT [PK_MGChooseAdvSlides] PRIMARY KEY CLUSTERED ([CASID] ASC),
    CONSTRAINT [FK_MGChooseAdvSlides_MGChooseAdv] FOREIGN KEY ([CAID]) REFERENCES [dbo].[MGChooseAdv] ([CAID])
);

