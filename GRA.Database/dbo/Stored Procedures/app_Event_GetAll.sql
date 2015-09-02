
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Event_GetAll] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT Code
		FROM dbo.Code
		WHERE CID = BranchID
		) AS Branch
FROM [Event]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY EventDate DESC
