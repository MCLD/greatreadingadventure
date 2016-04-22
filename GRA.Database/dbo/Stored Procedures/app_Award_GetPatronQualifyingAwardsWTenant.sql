
CREATE PROCEDURE [dbo].[app_Award_GetPatronQualifyingAwardsWTenant] @PID INT = 0,
	@TenID INT = 1
AS
SELECT award.*,
	patron.PID,
	patron.ProgID,
	patron.PrimaryLibrary,
	patron.District,
	patron.SchoolName,
	patron.TotalGoal,
	patron.Points
FROM Award award
INNER JOIN (
	SELECT pt.PID,
		pt.progID,
		pt.PrimaryLibrary,
		pt.District,
		pt.SchoolName,
		isnull(pt.GoalCache, - 1) AS TotalGoal,
		isnull((
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0)
				FROM PatronPoints pp
				WHERE pp.PID = pt.PID
				), 0) AS Points,
		@TenID AS TenID
	FROM Patron pt
	WHERE pt.PID = @PID
	) AS patron ON patron.TenID = award.TenID
	AND (
		award.ProgramID = patron.ProgID
		OR award.ProgramID = 0
		)
	AND (
		award.BranchID = patron.PrimaryLibrary
		OR award.BranchID = 0
		)
	AND (
		award.District = patron.District
		OR award.District = ''
		)
	AND (
		award.SchoolName = patron.SchoolName
		OR award.SchoolName = ''
		)
	AND (award.NumPoints <= patron.Points)
	AND (
		TotalGoal < 1
		OR award.GoalPercent <= (patron.points * 100) / TotalGoal
		)
	AND (
		BadgeList = ''
		OR dbo.fx_PatronBadgeCount(patron.PID, BadgeList) >= award.BadgesAchieved
		)
