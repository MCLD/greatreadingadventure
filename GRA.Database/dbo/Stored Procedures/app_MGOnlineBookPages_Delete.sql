
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_Delete] @OBPGID INT
AS
DECLARE @MGID INT;

SELECT @MGID = MGID
FROM [MGOnlineBookPages]
WHERE OBPGID = @OBPGID

DELETE
FROM [MGOnlineBookPages]
WHERE OBPGID = @OBPGID

EXEC app_MGOnlineBookPages_Reorder @MGID
