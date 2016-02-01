CREATE TABLE [dbo].[LibraryCrosswalk] (
    [ID]         INT          IDENTITY (1, 1) NOT NULL,
    [BranchID]   INT          NULL,
    [DistrictID] INT          NULL,
    [City]       VARCHAR (50) NULL,
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
    [BranchLink] NVARCHAR(255) NULL, 
    [BranchAddress] NVARCHAR(255) NULL, 
    [BranchTelephone] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_LibraryCrosswalk] PRIMARY KEY CLUSTERED ([ID] ASC)
);

