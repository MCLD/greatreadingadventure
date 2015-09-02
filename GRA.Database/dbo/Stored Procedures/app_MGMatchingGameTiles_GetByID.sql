
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_GetByID] @MAGTID INT
AS
SELECT *
FROM [MGMatchingGameTiles]
WHERE MAGTID = @MAGTID
