
/****** Object:  StoredProcedure [dbo].[app_ProgramGame_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGame_GetByID] @PGID INT
AS
SELECT *
FROM [ProgramGame]
WHERE PGID = @PGID
