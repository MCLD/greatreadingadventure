
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_Reorder] @MGID INT
AS
UPDATE MGOnlineBookPages
SET PageNumber = rowNumber
FROM MGOnlineBookPages
INNER JOIN (
	SELECT OBPGID,
		PageNumber,
		row_number() OVER (
			ORDER BY PageNumber ASC
			) AS rowNumber
	FROM MGOnlineBookPages
	WHERE MGID = @MGID
	) drRowNumbers ON drRowNumbers.OBPGID = MGOnlineBookPages.OBPGID
	AND MGID = @MGID
WHERE MGID = @MGID
