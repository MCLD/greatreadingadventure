
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_MoveUp] @OBPGID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@MGID INT

SELECT @CurrentRecordLocation = PageNumber,
	@MGID = MGID
FROM MGOnlineBookPages
WHERE OBPGID = @OBPGID

EXEC [dbo].[app_MGOnlineBookPages_Reorder] @MGID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = OBPGID
	FROM MGOnlineBookPages
	WHERE PageNumber = (@CurrentRecordLocation - 1)
		AND MGID = @MGID

	UPDATE MGOnlineBookPages
	SET PageNumber = @CurrentRecordLocation - 1
	WHERE OBPGID = @OBPGID

	UPDATE MGOnlineBookPages
	SET PageNumber = @CurrentRecordLocation
	WHERE OBPGID = @PreviousRecordID
END
