
CREATE PROCEDURE [dbo].[app_ProgramCodes_Generate] @start INT = 1,
	@end INT = 10000,
	@PID INT = 1
AS
-- generate x rows/numbers
--DECLARE @start INT = 1;
--DECLARE @end INT = 10000;
WITH numbers
AS (
	SELECT @start AS Number,
		NEWID() AS Code
	WHERE Substring(CONVERT(VARCHAR(36), NEWID()), 4, 20) NOT IN (
			SELECT ShortCode
			FROM ProgramCodes
			)
	
	UNION ALL
	
	SELECT number + 1,
		NEWID()
	FROM numbers
	WHERE number < @end
	)
INSERT INTO ProgramCodes (
	PID,
	CodeNumber,
	CodeValue,
	isUsed,
	DateCreated,
	DateUsed,
	PatronId,
	ShortCode
	)
SELECT @PID,
	Number,
	Code,
	0,
	GETDATE(),
	NULL,
	0,
	Substring(CONVERT(VARCHAR(36), Code), 4, 20)
FROM numbers
OPTION (MAXRECURSION 0)
