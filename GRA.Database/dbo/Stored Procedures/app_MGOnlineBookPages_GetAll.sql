
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_GetAll] @MGID INT = 0
AS
SELECT *,
	(
		SELECT isnull(Max(PageNumber), 1)
		FROM [MGOnlineBookPages]
		WHERE MGID = @MGID
		) AS MAX
FROM [MGOnlineBookPages]
WHERE MGID = @MGID
ORDER BY PageNumber
