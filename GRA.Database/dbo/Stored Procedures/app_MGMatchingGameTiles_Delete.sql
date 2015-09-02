
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_Delete] @MAGTID INT
AS
DELETE
FROM [MGMatchingGameTiles]
WHERE MAGTID = @MAGTID
