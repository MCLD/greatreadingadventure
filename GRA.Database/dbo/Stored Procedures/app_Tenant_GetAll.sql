
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Tenant_GetAll]
AS
SELECT *
FROM [Tenant]
ORDER BY LandingName
