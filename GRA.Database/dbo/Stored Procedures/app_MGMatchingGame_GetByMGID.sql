
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetByMGID]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGMatchingGame_GetByMGID] @MGID INT
AS
SELECT *
FROM MGMatchingGame
WHERE MGID = @MGID
