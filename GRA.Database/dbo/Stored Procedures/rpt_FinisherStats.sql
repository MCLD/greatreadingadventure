CREATE PROCEDURE [dbo].[rpt_FinisherStats] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

SELECT ProgID,
	pg.AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END AS Age,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) AS Male,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) AS Female,
	Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) AS Other,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'M'
				THEN 0
			WHEN 'O'
				THEN 0
			ELSE 1
			END) AS NA
FROM Patron
LEFT JOIN Programs pg ON ProgID = pg.PID --AND Patron.TenID = pg.TenID
LEFT OUTER JOIN (
	SELECT pp.[PID]
	FROM [PatronPoints] pp
	INNER JOIN [Patron] p ON p.[PID] = pp.[PID]
		AND p.[TenID] = @TenID
	INNER JOIN [Programs] prg ON prg.[PID] = p.[ProgID]
		AND prg.[TenID] = @TenID
	GROUP BY pp.[PID],
		prg.[CompletionPoints]
	HAVING SUM(pp.[NumPoints]) >= prg.[CompletionPoints]
	) pp ON Patron.[pid] = pp.[pid]
WHERE Patron.TenID = @TenID
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
	AND pp.[PID] IS NOT NULL
GROUP BY ProgID,
	AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END
ORDER BY ProgID,
	Age