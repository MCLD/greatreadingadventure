
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Tenant_Delete] @TenID INT
AS
DELETE
FROM [Tenant]
WHERE TenID = @TenID
