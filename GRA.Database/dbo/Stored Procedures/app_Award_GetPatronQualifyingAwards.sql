
CREATE PROCEDURE [dbo].[app_Award_GetPatronQualifyingAwards] @PID INT = 0
AS
SELECT a.*,
	p.PID,
	ProgID,
	PrimaryLibrary,
	p.District,
	p.SchoolName,
	Points
FROM Award a
INNER JOIN (
	SELECT PID,
		ProgID,
		PrimaryLibrary,
		District,
		SchoolName,
		isnull((
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0)
				FROM PatronPoints pp
				WHERE pp.PID = pt.PID
				), 0) AS Points,
		TenID
	FROM Patron pt
	WHERE pt.PID = @PID
	) AS p ON p.TenID = a.TenID
	AND (
		a.ProgramID = p.ProgID
		OR a.ProgramID = 0
		)
	AND (
		a.BranchID = p.PrimaryLibrary
		OR a.BranchID = 0
		)
	AND (
		a.District = p.District
		OR a.District = ''
		)
	AND (
		a.SchoolName = p.SchoolName
		OR a.SchoolName = ''
		)
	AND (a.NumPoints <= p.Points)
	AND (
		BadgeList = ''
		OR dbo.fx_PatronHasAllBadgesInList(p.PID, BadgeList) = 1
		)
