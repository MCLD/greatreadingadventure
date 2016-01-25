
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeGallery] @TenID INT,
	@A INT = 0,
	@B INT = 0,
	@C INT = 0,
	@L INT = 0
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT BID,
	UserName AS NAME
FROM Badge b
WHERE TenID = @TenID
	AND (
		b.BID IN (
			SELECT BID
			FROM BadgeAgeGrp A
			WHERE b.BID = A.BID
				AND A.CID = @A
			)
		OR @A = 0
		)
	AND (
		b.BID IN (
			SELECT BID
			FROM BadgeBranch B
			WHERE b.BID = B.BID
				AND B.CID = @B
			)
		OR @B = 0
		)
	AND (
		b.BID IN (
			SELECT BID
			FROM BadgeCategory C
			WHERE b.BID = C.BID
				AND C.CID = @C
			)
		OR @C = 0
		)
	AND (
		b.BID IN (
			SELECT BID
			FROM BadgeLocation L
			WHERE b.BID = L.BID
				AND L.CID = @l
			)
		OR @L = 0
		)
ORDER BY b.UserName
