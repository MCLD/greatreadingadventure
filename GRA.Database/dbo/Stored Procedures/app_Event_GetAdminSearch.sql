
CREATE PROCEDURE [dbo].[app_Event_GetAdminSearch] @startDate DATETIME,
	@endDate DATETIME,
	@branchID INT,
	@TenID INT = NULL
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
	AND (
		BranchID = @branchID
		OR @branchID = 0
		)
	AND (
		EventDate >= @startDate
		OR @startDate IS NULL
		)
	AND (
		EventDate <= @endDate
		OR @endDate IS NULL
		)
ORDER BY EventDate DESC
