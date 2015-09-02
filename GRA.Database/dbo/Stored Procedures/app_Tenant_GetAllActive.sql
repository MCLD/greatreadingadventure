
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Tenant_GetAllActive]
AS
SELECT *
FROM [Tenant]
WHERE isActiveFlag = 1
	AND isMasterFlag = 0
ORDER BY LandingName
