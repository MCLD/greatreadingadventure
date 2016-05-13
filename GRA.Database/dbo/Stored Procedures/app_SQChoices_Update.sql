
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SQChoices_Update] (
	@SQCID INT,
	@QID INT,
	@ChoiceOrder INT,
	@ChoiceText VARCHAR(255),
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
	@FldText3 TEXT
	)
AS
UPDATE SQChoices
SET QID = @QID
	--,ChoiceOrder =  @ChoiceOrder
	,
	ChoiceText = @ChoiceText,
	Score = @Score,
	JumpToQuestion = @JumpToQuestion,
	AskClarification = @AskClarification,
	ClarificationRequired = @ClarificationRequired,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE SQCID = @SQCID
