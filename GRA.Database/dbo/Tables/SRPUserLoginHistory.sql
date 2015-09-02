CREATE TABLE [dbo].[SRPUserLoginHistory] (
    [UIDLH]         INT           IDENTITY (1000, 1) NOT NULL,
    [UID]           NCHAR (10)    NULL,
    [SessionsID]    VARCHAR (128) NULL,
    [StartDateTime] DATETIME      NULL,
    [IP]            VARCHAR (50)  NULL,
    [MachineName]   VARCHAR (50)  NULL,
    [Browser]       VARCHAR (50)  NULL,
    [EndDateTime]   DATETIME      NULL,
    CONSTRAINT [PK_SRPUserLoginHistory] PRIMARY KEY CLUSTERED ([UIDLH] ASC)
);

