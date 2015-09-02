
/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_GetAll]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_RegistrationSettings_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [RegistrationSettings]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
