
CREATE PROCEDURE [dbo].[app_ProgramCodes_GetExportList] @PID INT = 1
AS
SELECT CodeNumber AS "Code Number",
	ShortCode AS "Code Value",
	CASE isUsed
		WHEN 1
			THEN 'Yes'
		ELSE 'No'
		END AS "Code Was Assigned",
	DateUsed AS "Date Used",
	p.FirstName AS "Assigned to First Name",
	p.LastName AS "Assigned to Last Name",
	p.Username AS "Assigned to Username"
FROM ProgramCodes pc
LEFT JOIN Patron p ON pc.PatronId = p.PID
WHERE pc.PID = @PID
ORDER BY PCID
