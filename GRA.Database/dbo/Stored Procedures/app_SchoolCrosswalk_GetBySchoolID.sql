
CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_GetBySchoolID] @ID INT = 0
AS
SELECT *
FROM SchoolCrosswalk
WHERE SchoolID = @ID
