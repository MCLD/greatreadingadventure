
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeTemplate_GetByID] @TID INT
AS
SELECT *
FROM [PrizeTemplate]
WHERE TID = @TID
