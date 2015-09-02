
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Survey_GetAllFinalized] @TenID INT = NULL
AS
SELECT *
FROM [Survey]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
	AND STATUS = 2
