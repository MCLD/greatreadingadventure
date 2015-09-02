
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_GetAll]    Script Date: 01/05/2015 14:43:22 ******/
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_GetAll] @MGID INT = 0
AS
SELECT *
FROM MGMatchingGameTiles
WHERE MGID = @MGID
