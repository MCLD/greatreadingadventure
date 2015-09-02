
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_Insert] (
	@TenID INT,
	@PID INT,
	@SID INT,
	@StartDate DATETIME,
	@EndDate DATETIME,
	@IsComplete BIT,
	@IsScorable BIT,
	@LastAnswered INT,
	@Score INT,
	@ScorePct DECIMAL,
	@Source VARCHAR(50),
	@SourceID INT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@SRID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SurveyResults (
		TenID,
		PID,
		SID,
		StartDate,
		EndDate,
		IsComplete,
		IsScorable,
		LastAnswered,
		Score,
		ScorePct,
		Source,
		SourceID,
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
		@TenID,
		@PID,
		@SID,
		@StartDate,
		@EndDate,
		@IsComplete,
		@IsScorable,
		@LastAnswered,
		@Score,
		@ScorePct,
		@Source,
		@SourceID,
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

	SELECT @SRID = SCOPE_IDENTITY()
END
