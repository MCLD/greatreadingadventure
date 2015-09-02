
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_MoveUp]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_MoveUp] @CASID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@MGID INT,
	@Diff INT,
	@CAID INT

SELECT @CurrentRecordLocation = StepNumber,
	@CAID = CAID,
	@MGID = MGID,
	@Diff = Difficulty
FROM MGChooseAdvSlides
WHERE CASID = @CASID

EXEC [dbo].[app_MGChooseAdvSlides_Reorder] @MGID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = CASID
	FROM MGChooseAdvSlides
	WHERE StepNumber = (@CurrentRecordLocation - 1)
		AND MGID = @MGID
		AND Difficulty = @Diff

	UPDATE MGChooseAdvSlides
	SET StepNumber = @CurrentRecordLocation - 1
	WHERE CASID = @CASID

	UPDATE MGChooseAdvSlides
	SET StepNumber = @CurrentRecordLocation
	WHERE CASID = @PreviousRecordID
END
