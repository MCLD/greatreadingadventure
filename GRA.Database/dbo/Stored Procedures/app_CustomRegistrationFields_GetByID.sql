
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CustomRegistrationFields_GetByID] @TenID INT
AS
SELECT *
FROM [CustomRegistrationFields]
WHERE TenID = @TenID
