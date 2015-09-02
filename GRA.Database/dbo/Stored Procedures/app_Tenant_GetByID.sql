
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Tenant_GetByID] @TenID INT
AS
SELECT *
FROM [Tenant]
WHERE TenID = @TenID
