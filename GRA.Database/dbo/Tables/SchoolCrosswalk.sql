CREATE TABLE [dbo].[SchoolCrosswalk] (
    [ID]         INT          IDENTITY (1, 1) NOT NULL,
    [SchoolID]   INT          NULL,
    [SchTypeID]  INT          NULL,
    [DistrictID] INT          NULL,
    [City]       VARCHAR (50) NULL,
    [MinGrade]   INT          NULL,
    [MaxGrade]   INT          NULL,
    [MinAge]     INT          NULL,
    [MaxAge]     INT          NULL,
    [TenID]      INT          NULL,
    [FldInt1]    INT          NULL,
    [FldInt2]    INT          NULL,
    [FldInt3]    INT          NULL,
    [FldBit1]    BIT          NULL,
    [FldBit2]    BIT          NULL,
    [FldBit3]    BIT          NULL,
    [FldText1]   TEXT         NULL,
    [FldText2]   TEXT         NULL,
    [FldText3]   TEXT         NULL,
    CONSTRAINT [PK_SchoolCrosswalk] PRIMARY KEY CLUSTERED ([ID] ASC)
);

