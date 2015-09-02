
-- Gets all records from the 'SRPGroups' table.
CREATE PROCEDURE [dbo].[cbspSRPGroups_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [dbo].[SRPGroups]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
