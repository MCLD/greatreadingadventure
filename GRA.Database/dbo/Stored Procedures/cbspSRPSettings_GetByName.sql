
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_GetByName]    Script Date: 01/05/2015 14:43:27 ******/
-- Gets a record from the 'SRPSettings' table using the primary key value.
CREATE PROCEDURE [dbo].[cbspSRPSettings_GetByName] @Name VARCHAR(50)
AS
SELECT *
FROM [dbo].[SRPSettings]
WHERE [Name] = @Name
