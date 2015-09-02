
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Minigame_GetByID] @MGID INT
AS
SELECT *
FROM [Minigame]
WHERE MGID = @MGID
