
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGame_GetByID] @MAGID INT
AS
SELECT *
FROM [MGMatchingGame]
WHERE MAGID = @MAGID
