
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Minigame_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Minigame]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
