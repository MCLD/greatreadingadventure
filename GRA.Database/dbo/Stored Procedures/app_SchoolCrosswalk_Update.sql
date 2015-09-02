
CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_Update] (
	@ID INT,
	@SchoolID INT,
	@SchTypeID INT,
	@DistrictID INT,
	@City VARCHAR(50),
	@MinGrade INT,
	@MaxGrade INT,
	@MinAge INT,
	@MaxAge INT,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = ''
	)
AS
UPDATE SchoolCrosswalk
SET SchoolID = @SchoolID,
	SchTypeID = @SchTypeID,
	DistrictID = @DistrictID,
	City = @City,
	MinGrade = @MinGrade,
	MaxGrade = @MaxGrade,
	MinAge = @MinAge,
	MaxAge = @MaxAge,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE ID = @ID
	AND TenID = @TenID
