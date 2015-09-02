
CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_GetByID] @ID INT
AS
SELECT *
FROM [SchoolCrosswalk]
WHERE ID = @ID
