
/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_Delete]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_RegistrationSettings_Delete] @RID INT
AS
DELETE
FROM [RegistrationSettings]
WHERE RID = @RID
