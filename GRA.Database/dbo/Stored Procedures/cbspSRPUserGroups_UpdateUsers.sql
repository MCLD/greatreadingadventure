
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_UpdateUsers]    Script Date: 01/05/2015 14:43:28 ******/
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_UpdateUsers] @GID INT,
	@UID_LIST VARCHAR(4000),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.SRPUserGroups
WHERE GID = @GID
	AND UID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@UID_LIST)
		)

INSERT INTO dbo.SRPUserGroups
SELECT UID,
	@GID,
	getdate(),
	@ActionUsername
FROM dbo.SRPUser
WHERE UID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@UID_LIST)
		)
	AND UID NOT IN (
		SELECT UID
		FROM dbo.SRPUserGroups
		WHERE GID = @GID
		)
	AND IsDeleted = 0
