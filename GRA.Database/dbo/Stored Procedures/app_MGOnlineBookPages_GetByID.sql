
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_GetByID] @OBPGID INT
AS
SELECT *
FROM [MGOnlineBookPages]
WHERE OBPGID = @OBPGID
ORDER BY PageNumber
