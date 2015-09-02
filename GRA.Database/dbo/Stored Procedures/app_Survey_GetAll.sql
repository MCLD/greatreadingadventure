
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Survey_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Survey]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
