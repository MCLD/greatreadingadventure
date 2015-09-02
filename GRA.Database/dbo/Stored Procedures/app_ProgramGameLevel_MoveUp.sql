
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_MoveUp]    Script Date: 01/05/2015 14:43:25 ******/
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_MoveUp] @PGLID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@PGID INT

SELECT @CurrentRecordLocation = LevelNumber,
	@PGID = PGID
FROM ProgramGameLevel
WHERE PGLID = @PGLID

EXEC [dbo].[app_ProgramGameLevel_Reorder] @PGID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = PGLID
	FROM ProgramGameLevel
	WHERE LevelNumber = (@CurrentRecordLocation - 1)
		AND PGID = @PGID

	UPDATE ProgramGameLevel
	SET LevelNumber = @CurrentRecordLocation - 1
	WHERE PGLID = @PGLID

	UPDATE ProgramGameLevel
	SET LevelNumber = @CurrentRecordLocation
	WHERE PGLID = @PreviousRecordID
END
