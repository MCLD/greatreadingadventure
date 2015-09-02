
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SQChoices_Insert] (
	@QID INT,
	@ChoiceOrder INT,
	@ChoiceText VARCHAR(50),
	@Score INT,
	@JumpToQuestion INT,
	@AskClarification BIT,
	@ClarificationRequired BIT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@SQCID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SQChoices (
		QID,
		ChoiceOrder,
		ChoiceText,
		Score,
		JumpToQuestion,
		AskClarification,
		ClarificationRequired,
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
		@QID,
		(
			SELECT isnull(Max(ChoiceOrder), 0) + 1
			FROM SQChoices
			WHERE QID = @QID
			),
		@ChoiceText,
		@Score,
		@JumpToQuestion,
		@AskClarification,
		@ClarificationRequired,
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

	SELECT @SQCID = SCOPE_IDENTITY()
END
