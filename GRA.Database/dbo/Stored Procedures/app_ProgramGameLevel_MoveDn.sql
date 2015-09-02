
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_MoveDn]    Script Date: 01/05/2015 14:43:25 ******/
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_MoveDn] @PGLID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@PGID INT

SELECT @CurrentRecordLocation = LevelNumber,
	@PGID = PGID
FROM ProgramGameLevel
WHERE PGLID = @PGLID

EXEC [dbo].[app_ProgramGameLevel_Reorder] @PGID

IF @CurrentRecordLocation < (
		SELECT MAX(LevelNumber)
		FROM ProgramGameLevel
		WHERE PGID = @PGID
		)
BEGIN
	SELECT @NextRecordID = PGLID
	FROM ProgramGameLevel
	WHERE LevelNumber = (@CurrentRecordLocation + 1)
		AND PGID = @PGID

	UPDATE ProgramGameLevel
	SET LevelNumber = @CurrentRecordLocation + 1
	WHERE PGLID = @PGLID

	UPDATE ProgramGameLevel
	SET LevelNumber = @CurrentRecordLocation
	WHERE PGLID = @NextRecordID
END
