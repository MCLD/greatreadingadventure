
CREATE PROCEDURE [dbo].[app_SurveyQuestion_Insert] (
	@SID INT,
	@QNumber INT,
	@QType INT,
	@QName VARCHAR(150),
	@QText TEXT,
	@DisplayControl INT,
	@DisplayDirection INT,
	@IsRequired BIT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@QID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SurveyQuestion (
		SID,
		QNumber,
		QType,
		QName,
		QText,
		DisplayControl,
		DisplayDirection,
		IsRequired,
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
		@SID,
		(
			SELECT isnull(Max(QNumber), 0) + 1
			FROM SurveyQuestion
			WHERE SID = @SID
			) --@QNumber
		,
		@QType,
		@QName,
		@QText,
		@DisplayControl,
		@DisplayDirection,
		@IsRequired,
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

	SELECT @QID = SCOPE_IDENTITY()
END
