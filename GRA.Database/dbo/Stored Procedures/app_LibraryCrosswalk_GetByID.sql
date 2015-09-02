
CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_GetByID] @ID INT
AS
SELECT *
FROM [LibraryCrosswalk]
WHERE ID = @ID
