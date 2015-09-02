
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_UpdateBadgeAgeGroups] @BID INT,
	@TenID INT,
	@CID_LIST VARCHAR(4000)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.BadgeAgeGrp
WHERE BID = @BID
	AND CID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)

INSERT INTO dbo.BadgeAgeGrp
SELECT @BID,
	CID
FROM dbo.Code
WHERE CID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)
	AND CID NOT IN (
		SELECT CID
		FROM dbo.BadgeAgeGrp
		WHERE BID = @BID
		)
