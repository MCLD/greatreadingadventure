
CREATE PROCEDURE [dbo].[rpt_TenantReport] @IncSummary BIT = 0
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

SELECT t.NAME AS Organization,
	ISNULL((
			SELECT COUNT(1)
			FROM Programs x
			WHERE x.TenID = t.TenID
			), 0) AS [# Programs],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
			), 0) AS [# Patrons],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
				AND [dbo].[fx_IsFinisher](x.PID, x.ProgID) = 1
			), 0) AS [# Finishers],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronBadges x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
			), 0) AS [# Badges],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
				AND Gender = 'M'
			), 0) AS [Male Participation],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
				AND Gender = 'F'
			), 0) AS [Female Participation],
	ISNULL((
			SELECT Sum(NumPoints)
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND x.isReading = 1
			), 0) AS [# Reading Points],
	ISNULL((
			SELECT Sum(NumPoints)
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
			), 0) AS [# Total Points],
	ISNULL((
			SELECT Sum(dbo.fx_ConvertPoints(y.ProgID, isnull(NumPoints, 0), 3))
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND x.isReading = 1
			), 0) AS [# Reading Minutes]
FROM Tenant t

UNION

SELECT 'TOTAL: ',
	ISNULL((
			SELECT COUNT(1)
			FROM Programs x
			), 0) AS [# Programs],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			), 0) AS [# Patrons],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE [dbo].[fx_IsFinisher](x.PID, x.ProgID) = 1
			), 0) AS [# Finishers],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronBadges x ON y.PID = x.PID
			), 0) AS [# Badges],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE Gender = 'M'
			), 0) AS [Male Participation],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE Gender = 'F'
			), 0) AS [Female Participation],
	ISNULL((
			SELECT Sum(NumPoints)
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE x.isReading = 1
			), 0) AS [# Reading Points],
	ISNULL((
			SELECT Sum(NumPoints)
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			), 0) AS [# Total Points],
	ISNULL((
			SELECT Sum(dbo.fx_ConvertPoints(y.ProgID, isnull(NumPoints, 0), 3))
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE x.isReading = 1
			), 0) AS [# Reading Minutes]
WHERE @IncSummary = 1
