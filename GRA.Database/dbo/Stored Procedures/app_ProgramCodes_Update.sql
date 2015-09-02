
CREATE PROCEDURE [dbo].[app_ProgramCodes_Update] (
	@PCID INT,
	@PID INT,
	@CodeNumber INT,
	@CodeValue UNIQUEIDENTIFIER,
	@ShortCode VARCHAR(20) = '',
	@isUsed BIT,
	@DateCreated DATETIME,
	@DateUsed DATETIME,
	@PatronId INT
	)
AS
UPDATE ProgramCodes
SET PID = @PID,
	CodeNumber = @CodeNumber,
	CodeValue = @CodeValue,
	ShortCode = @ShortCode,
	isUsed = @isUsed,
	DateCreated = @DateCreated,
	DateUsed = @DateUsed,
	PatronId = @PatronId
WHERE PCID = @PCID
