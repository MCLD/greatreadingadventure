
CREATE PROCEDURE [dbo].[app_Event_GetUpcomingDisplay] @startDate DATETIME = NULL,
	@endDate DATETIME = NULL,
	@systemID INT = 0,
	@branchID INT = 0,
	@searchText NVARCHAR(255) = NULL,
	@TenID INT = NULL
AS
SELECT e.*,
	c.[Code] AS [Branch],
	lc.[BranchLink],
	lc.[BranchAddress],
	lc.[BranchTelephone]
FROM [Event] e
LEFT OUTER JOIN [Code] c ON e.[BranchID] = c.[CID]
LEFT OUTER JOIN [LibraryCrosswalk] lc ON e.[BranchID] = lc.[BranchID]
WHERE (
		e.[BranchID] = @branchID
		OR @branchID = 0
		)
	AND (
		lc.[DistrictID] = @systemID
		OR @systemID = 0
		)
	AND (
		e.[EventDate] >= @startDate
		OR (
			e.[EndDate] IS NOT NULL
			AND e.[EndDate] >= @startDate
			)
		OR @startDate IS NULL
		)
	AND (
		e.[EventDate] <= @endDate
		OR (
			e.[EndDate] IS NOT NULL
			AND e.[EndDate] <= @endDate
			)
		OR @endDate IS NULL
		)
	AND (
		dateadd(d, 1, e.[EventDate]) >= GETDATE()
		OR (
			dateadd(d, 1, e.[EndDate]) >= GETDATE()
			AND e.[EndDate] IS NOT NULL
			)
		)
	AND (
		e.[TenID] = @TenID
		OR @TenID IS NULL
		)
	AND e.[HiddenFromPublic] != 1
	AND (
		(
			e.[EventTitle] LIKE @searchText
			OR e.[HTML] LIKE @searchText
			)
		OR @searchText IS NULL
		)
ORDER BY e.[EventDate] ASC,
	e.[EventTitle]
