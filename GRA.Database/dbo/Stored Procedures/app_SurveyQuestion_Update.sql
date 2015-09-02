
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SurveyQuestion_Update] (
	@QID INT,
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
	@FldText3 TEXT
	)
AS
UPDATE SurveyQuestion
SET SID = @SID,
	QNumber = @QNumber,
	QType = @QType,
	QName = @QName,
	QText = @QText,
	DisplayControl = @DisplayControl,
	DisplayDirection = @DisplayDirection,
	IsRequired = @IsRequired,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE QID = @QID
