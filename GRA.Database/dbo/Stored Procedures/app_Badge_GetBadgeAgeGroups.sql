
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeAgeGroups] @BID INT,
	@TenID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @CTID INT

SELECT @CTID = CTID
FROM dbo.CodeType
WHERE TenID = @TenID
	AND CodeTypeName = 'Badge Age Group'

SELECT @BID AS BID,
	c.CID,
	c.Code AS NAME, --c.CTID,
	CASE 
		WHEN bb.BID IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM (
	SELECT *
	FROM dbo.BadgeAgeGrp
	WHERE BID = @BID
	) AS bb
RIGHT JOIN (
	SELECT *
	FROM Code
	WHERE TenID = @TenID
		AND CTID = @CTID
	) AS c ON bb.CID = c.CID
ORDER BY c.Code
