
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_Insert] (
	@SRID INT,
	@TenID INT,
	@PID INT,
	@SID INT,
	@QID INT,
	@SQMLID INT,
	@DateAnswered DATETIME,
	@QType INT,
	@FreeFormAnswer TEXT,
	@ClarificationText TEXT,
	@ChoiceAnswerIDs VARCHAR(2000),
	@ChoiceAnswerText TEXT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@SAID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SurveyAnswers (
		SRID,
		TenID,
		PID,
		SID,
		QID,
		SQMLID,
		DateAnswered,
		QType,
		FreeFormAnswer,
		ClarificationText,
		ChoiceAnswerIDs,
		ChoiceAnswerText,
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
		@SRID,
		@TenID,
		@PID,
		@SID,
		@QID,
		@SQMLID,
		@DateAnswered,
		@QType,
		@FreeFormAnswer,
		@ClarificationText,
		@ChoiceAnswerIDs,
		@ChoiceAnswerText,
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

	SELECT @SAID = SCOPE_IDENTITY()
END
