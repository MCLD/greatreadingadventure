
CREATE PROCEDURE [dbo].[rpt_TenantSummaryReport] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = 0
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

SELECT ISNULL((
			SELECT Value
			FROM SRPSettings x
			WHERE x.TenID = t.TenID
				AND NAME = 'SysName'
			), 'Summer Reading Program') AS SystemName,
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
				AND (
					x.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					x.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(x.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(x.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Patrons],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronBadges x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Badges],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN GamePlayStats x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Games Played]
	--, ISNULL((select COUNT(1) from Patron y join GamePlayStats x on y.PID = x.PID where y.TenID = t.TenID  and x.CompletedPlay  = 1),0) as [# Games Completed]
	,
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronBookLists x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND HasReadFlag = 1
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Book Lists Completed],
	ISNULL((
			SELECT COUNT(1)
			FROM Programs y
			INNER JOIN ProgramCodes x ON y.PID = x.PID
			INNER JOIN Patron z ON x.PatronId = z.PID
			WHERE y.TenID = t.TenID
				AND isUsed = 1
				AND (
					z.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					z.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(z.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(z.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) + ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronPrizes x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND RedeemedFlag = 1
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Prizes Claimed],
	ISNULL((
			SELECT Sum(dbo.fx_ConvertPoints(y.ProgID, isnull(NumPoints, 0), 3))
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND x.isReading = 1
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Reading Minutes],
	ISNULL((
			SELECT COUNT(DISTINCT Title)
			FROM Patron y
			INNER JOIN PatronReadingLog x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Titles Read],
	ISNULL((
			SELECT COUNT(DISTINCT Title)
			FROM Patron y
			INNER JOIN PatronReadingLog x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND HasReview = 1
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Titles Reviewed]
FROM Tenant t
WHERE t.TenID = @TenID
	-- AND (x.ProgID = @ProgID or @ProgId is null) AND (x.PrimaryLibrary = @BranchID or @BranchID is null) AND (rtrim(ltrim(isnull(x.SchoolName,''))) = @School or @School is null) AND (rtrim(ltrim(isnull(x.District,''))) = @LibSys or @LibSys is null)
	-- AND (y.ProgID = @ProgID or @ProgId is null) AND (y.PrimaryLibrary = @BranchID or @BranchID is null) AND (rtrim(ltrim(isnull(y.SchoolName,''))) = @School or @School is null) AND (rtrim(ltrim(isnull(y.District,''))) = @LibSys or @LibSys is null)
