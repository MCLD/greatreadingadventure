CREATE TABLE [dbo].[TenantInitData] (
    [InitID]      INT          IDENTITY (1, 1) NOT NULL,
    [IntitType]   VARCHAR (50) NULL,
    [DestTID]     INT          NULL,
    [SrcPK]       INT          NULL,
    [DateCreated] DATETIME     CONSTRAINT [DF_TenantInitData_DateCreated] DEFAULT (getdate()) NULL,
    [DstPK]       INT          NULL,
    CONSTRAINT [PK_TenantInitData] PRIMARY KEY CLUSTERED ([InitID] ASC)
);

