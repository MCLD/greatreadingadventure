
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGame_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [ProgramGame]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
