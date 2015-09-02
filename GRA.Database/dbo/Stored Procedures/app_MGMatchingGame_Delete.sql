
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGame_Delete] @MAGID INT
AS
DELETE
FROM [MGMatchingGame]
WHERE MAGID = @MAGID
