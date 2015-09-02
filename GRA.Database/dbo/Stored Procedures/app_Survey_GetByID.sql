
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Survey_GetByID] @SID INT
AS
SELECT *
FROM [Survey]
WHERE SID = @SID
