
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_DeleteByPrimaryKey]    Script Date: 01/05/2015 14:43:27 ******/
-- Deletes a record from the 'SRPSettings' table using the primary key value.
CREATE PROCEDURE [dbo].[cbspSRPSettings_DeleteByPrimaryKey] @SID INT
AS
DELETE
FROM [dbo].[SRPSettings]
WHERE [SID] = @SID
