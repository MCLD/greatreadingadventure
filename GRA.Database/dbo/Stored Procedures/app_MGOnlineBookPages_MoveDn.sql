
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_MoveDn] @OBPGID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@MGID INT

SELECT @CurrentRecordLocation = PageNumber,
	@MGID = MGID
FROM MGOnlineBookPages
WHERE OBPGID = @OBPGID

EXEC [dbo].[app_MGOnlineBookPages_Reorder] @MGID

IF @CurrentRecordLocation < (
		SELECT MAX(PageNumber)
		FROM MGOnlineBookPages
		WHERE MGID = @MGID
		)
BEGIN
	SELECT @NextRecordID = OBPGID
	FROM MGOnlineBookPages
	WHERE PageNumber = (@CurrentRecordLocation + 1)
		AND MGID = @MGID

	UPDATE MGOnlineBookPages
	SET PageNumber = @CurrentRecordLocation + 1
	WHERE OBPGID = @OBPGID

	UPDATE MGOnlineBookPages
	SET PageNumber = @CurrentRecordLocation
	WHERE OBPGID = @NextRecordID
END
