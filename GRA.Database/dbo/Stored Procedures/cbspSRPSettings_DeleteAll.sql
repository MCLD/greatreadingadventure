
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_DeleteAll]    Script Date: 01/05/2015 14:43:27 ******/
-- Deletes all records from the 'SRPSettings' table.
CREATE PROCEDURE [dbo].[cbspSRPSettings_DeleteAll]
AS
DELETE
FROM [dbo].[SRPSettings]
