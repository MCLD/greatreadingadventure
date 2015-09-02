
CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_Delete] @ID INT
AS
DELETE
FROM [SchoolCrosswalk]
WHERE ID = @ID
