
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_GetAll]    Script Date: 01/05/2015 14:43:27 ******/
-- Gets all records from the 'SRPSettings' table.
CREATE PROCEDURE [dbo].[cbspSRPSettings_GetAll]
AS
SELECT *
FROM [dbo].[SRPSettings]
