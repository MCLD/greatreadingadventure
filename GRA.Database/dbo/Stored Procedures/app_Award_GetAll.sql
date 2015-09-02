
CREATE PROCEDURE [dbo].[app_Award_GetAll] @TenID INT = NULL
AS
SELECT *,
	isnull((
			SELECT AdminName
			FROM Badge
			WHERE BID = BadgeId
			), '') AS BadgeName,
	isnull((
			SELECT Code
			FROM Code
			WHERE CID = BranchID
			), '') AS Branch,
	isnull((
			SELECT AdminName
			FROM Programs
			WHERE PID = ProgramId
			), '') AS Program,
	isnull((
			SELECT Code
			FROM Code
			WHERE CID = District
			), '') AS DistrictName,
	isnull((
			SELECT Code
			FROM Code
			WHERE CID = SchoolName
			), '') AS SchName
FROM [Award]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY AwardName
