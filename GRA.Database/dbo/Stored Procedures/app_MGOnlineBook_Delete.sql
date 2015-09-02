
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBook_Delete] @OBID INT
AS
DELETE
FROM [MGOnlineBook]
WHERE OBID = @OBID
