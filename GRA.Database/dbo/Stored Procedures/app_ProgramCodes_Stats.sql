
CREATE PROCEDURE [dbo].[app_ProgramCodes_Stats] @PID INT = 1
AS
SELECT isnull((
			SELECT COUNT(*)
			FROM ProgramCodes
			WHERE PID = @PID
			), 0) AS TotalCodes,
	isnull((
			SELECT COUNT(*)
			FROM ProgramCodes
			WHERE PID = @PID
				AND isUsed = 1
			), 0) AS UsedCodes,
	isnull((
			SELECT COUNT(*)
			FROM ProgramCodes
			WHERE PID = @PID
				AND isUsed = 0
			), 0) AS RemainingCodes
	--,isnull((select Top 1 convert(varchar(64),CodeValue) from ProgramCodes where PID = @PID and isUsed=1 order by PCID desc),'') as LastUsedCode
	,
	isnull((
			SELECT TOP 1 ShortCode
			FROM ProgramCodes
			WHERE PID = @PID
				AND isUsed = 1
			ORDER BY PCID DESC
			), '') AS LastUsedCode
