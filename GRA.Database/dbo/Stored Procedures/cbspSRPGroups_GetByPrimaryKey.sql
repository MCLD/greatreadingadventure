
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_GetByPrimaryKey]    Script Date: 01/05/2015 14:43:27 ******/
-- Gets a record from the 'SRPGroups' table using the primary key value.
CREATE PROCEDURE [dbo].[cbspSRPGroups_GetByPrimaryKey] @GID INT
AS
SELECT *
FROM [dbo].[SRPGroups]
WHERE [GID] = @GID
