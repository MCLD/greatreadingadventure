
/****** Object:  StoredProcedure [dbo].[app_ProgramGame_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_ProgramGame_Delete] @PGID INT
AS
DELETE ProgramGameLevel
WHERE PGID = @PGID

DELETE
FROM [ProgramGame]
WHERE PGID = @PGID
