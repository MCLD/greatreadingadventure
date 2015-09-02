
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBook_GetByID] @OBID INT
AS
SELECT *
FROM [MGOnlineBook]
WHERE OBID = @OBID
