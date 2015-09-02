
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Programs_Delete] @PID INT,
	@PatronProgram INT = 0,
	@PrizeProgram INT = 0,
	@OfferProgram INT = 0,
	@BookListProgram INT = 0
AS
UPDATE Patron
SET ProgID = @PatronProgram
WHERE ProgID = @PID

UPDATE PrizeTemplate
SET ProgID = @PatronProgram
WHERE ProgID = @PID

UPDATE Offer
SET ProgramId = @PatronProgram
WHERE ProgramId = @PID

UPDATE BookList
SET ProgID = @PatronProgram
WHERE ProgID = @PID

DELETE
FROM ProgramCodes
WHERE PID = @PID

DELETE
FROM ProgramGamePointConversion
WHERE PGID = @PID

--DELETE from ProgramCodes where PID = @PID
DECLARE @TenID INT

SELECT @TenID = TenID
FROM Programs
WHERE PID = @PID

DELETE
FROM [Programs]
WHERE PID = @PID

EXEC [app_Programs_Reorder] @TenID
