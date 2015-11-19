
CREATE PROCEDURE [dbo].[app_Event_GetUpcomingDisplay] @startDate DATETIME = NULL,
	@endDate DATETIME = NULL,
	@branchID INT = 0,
	@TenID INT = NULL
AS
SELECT *,
	(
		SELECT [Description]
		FROM dbo.Code
		WHERE CID = BranchID
		) AS Branch
FROM [Event]
WHERE (
		BranchID = @branchID
		OR @branchID = 0
		)
	AND (
		EventDate >= @startDate
		OR (
			EndDate IS NOT NULL
			AND EndDate >= @startDate
			)
		OR @startDate IS NULL
		)
	AND (
		EventDate <= @endDate
		OR (
			EndDate IS NOT NULL
			AND EndDate <= @endDate
			)
		OR @endDate IS NULL
		)
	AND (
		dateadd(d, 1, EventDate) >= GETDATE()
		OR (
			dateadd(d, 1, EndDate) >= GETDATE()
			AND EndDate IS NOT NULL
			)
		)
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY EventDate ASC,
	EventTitle
