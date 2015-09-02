
CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_Insert] (
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
	@FldText3 TEXT = '',
	@ID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SchoolCrosswalk (
		SchoolID,
		SchTypeID,
		DistrictID,
		City,
		MinGrade,
		MaxGrade,
		MinAge,
		MaxAge,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3
		)
	VALUES (
		@SchoolID,
		@SchTypeID,
		@DistrictID,
		@City,
		@MinGrade,
		@MaxGrade,
		@MinAge,
		@MaxAge,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3
		)

	SELECT @ID = SCOPE_IDENTITY()
END
