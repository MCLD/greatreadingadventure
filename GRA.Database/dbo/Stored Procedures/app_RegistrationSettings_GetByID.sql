
/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_GetByID]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_RegistrationSettings_GetByID] @TenID INT
AS
SELECT *
FROM [RegistrationSettings]
WHERE TenID = @TenID
