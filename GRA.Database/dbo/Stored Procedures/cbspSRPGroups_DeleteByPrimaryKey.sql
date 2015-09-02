
-- Deletes a record from the 'SRPGroups' table using the primary key value.
CREATE PROCEDURE [dbo].[cbspSRPGroups_DeleteByPrimaryKey] @GID INT,
	@TenID INT = NULL
AS
DELETE
FROM [dbo].[SRPGroups]
WHERE [GID] = @GID
	AND TenID = @TenID
