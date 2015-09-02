
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_GetByID]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_GetByID] @PGLID INT
AS
SELECT *
FROM [ProgramGameLevel]
WHERE PGLID = @PGLID
