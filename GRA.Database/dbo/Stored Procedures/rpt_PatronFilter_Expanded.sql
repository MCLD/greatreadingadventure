
CREATE PROCEDURE [dbo].[rpt_PatronFilter_Expanded] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL,
	@Points INT = 0,
	@PointType INT = - 1,
	@StartDate DATETIME = NULL,
	@EndDate DATETIME = NULL
AS
SET ARITHABORT OFF
SET ANSI_WARNINGS OFF

SELECT DISTINCT Patron.PID
FROM Patron
WHERE TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
	AND (
		@Points = 0
		OR (
			SELECT isNull(SUM(NumPoints), 0)
			FROM PatronPoints
			WHERE (
					AwardReasonCd = @PointType
					OR @PointType < 0
					)
				AND (
					AwardDate >= @StartDate
					OR @StartDate IS NULL
					)
				AND (
					AwardDate <= @EndDate
					OR @EndDate IS NULL
					)
				AND PatronPoints.PID = Patron.PID
			) >= @Points
		)
