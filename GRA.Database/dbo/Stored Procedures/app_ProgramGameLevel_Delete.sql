
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Delete]    Script Date: 01/05/2015 14:43:24 ******/
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_Delete] @PGLID INT
AS
DECLARE @PGID INT

SELECT @PGID = PGID
FROM [ProgramGameLevel]
WHERE PGLID = @PGLID

DELETE
FROM [ProgramGameLevel]
WHERE PGLID = @PGLID

EXEC [app_ProgramGameLevel_Reorder] @PGID
