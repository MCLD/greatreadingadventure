
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_ResetPassword]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUser_ResetPassword] @UID INT,
	@Password VARCHAR(50),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPUser
SET Password = @Password,
	LastPasswordReset = getdate(),
	LastModDate = getdate(),
	LastModUser = @ActionUsername
WHERE UID = @UID

SELECT *
FROM dbo.SRPUser
WHERE UID = @UID

EXEC cbspSRPUser_GetAllPermissions @UID
