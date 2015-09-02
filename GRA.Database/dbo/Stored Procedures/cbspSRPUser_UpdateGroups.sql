
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_UpdateGroups]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_UpdateGroups] @UID INT,
	@GID_LIST VARCHAR(4000),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.SRPUserGroups
WHERE UID = @UID
	AND GID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@GID_LIST)
		)

INSERT INTO dbo.SRPUserGroups
SELECT @UID,
	GID,
	getdate(),
	@ActionUsername
FROM dbo.SRPGroups
WHERE GID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@GID_LIST)
		)
	AND GID NOT IN (
		SELECT GID
		FROM dbo.SRPUserGroups
		WHERE UID = @UID
		)
