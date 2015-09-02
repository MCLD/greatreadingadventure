
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_DeleteAll]    Script Date: 01/05/2015 14:43:27 ******/
-- Deletes all records from the 'SRPGroups' table.
CREATE PROCEDURE [dbo].[cbspSRPGroups_DeleteAll]
AS
DELETE
FROM [dbo].[SRPGroups]
