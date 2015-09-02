
CREATE PROCEDURE [dbo].[rpt_PrizeRecipients] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SELECT AdminName,
	FirstName,
	LastName,
	Username,
	EmailAddress,
	PrizeName,
	CASE RedeemedFlag
		WHEN 1
			THEN 'Yes'
		ELSE 'No'
		END PrizeRedeemed
FROM PatronPrizes r
LEFT JOIN Patron p ON p.PID = r.PID
LEFT JOIN Programs pg ON p.ProgID = pg.PID
WHERE p.TenID = @TenID
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
--AND [dbo].[fx_IsFinisher](Patron.PID, Pg.PID) = 1
ORDER BY AdminName,
	FirstName,
	LastName,
	RedeemedFlag DESC
