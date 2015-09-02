
CREATE PROCEDURE [dbo].[app_Event_GetEventList] @List VARCHAR(2000) = '',
	@TenID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @Now DATETIME

SELECT @Now = GETDATE()

SELECT CASE 
		WHEN EventDate <= @Now
			AND EndDate IS NULL
			THEN 'Expired '
		WHEN EndDate IS NOT NULL
			AND EndDate <= @Now
			THEN 'Expired '
		WHEN EventDate <= @Now
			AND EndDate >= @Now
			THEN ''
		WHEN EventDate >= @Now
			THEN ''
		ELSE ''
		END AS Expired,
	*,
	isnull(Code, '') AS Branch
FROM Event
LEFT JOIN Code ON BranchID = CID
WHERE Event.TenID = @TenID
	AND EID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@List)
		)
ORDER BY Expired,
	EventDate
