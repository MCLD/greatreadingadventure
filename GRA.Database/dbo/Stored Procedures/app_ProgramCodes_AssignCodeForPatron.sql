
CREATE PROCEDURE [dbo].[app_ProgramCodes_AssignCodeForPatron] (
	@PID INT,
	@PatronId INT
	)
AS
DECLARE @PCID INT

SELECT TOP 1 @PCID = PCID
FROM ProgramCodes
WHERE PID = @PID
	AND isUsed = 0
ORDER BY PCID

UPDATE ProgramCodes
SET isUsed = 1,
	DateUsed = GETDATE(),
	PatronId = @PatronId
WHERE PCID = @PCID

SELECT *,
	0
FROM ProgramCodes
WHERE PCID = @PCID

UNION

SELECT *,
	1
FROM ProgramCodes
WHERE PCID = @PCID
