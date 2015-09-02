
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Login]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUser_Login] @UserName VARCHAR(50),
	@SessionId VARCHAR(128) = 'N/A',
	@IP VARCHAR(50) = 'N/A',
	@MachineName VARCHAR(50) = 'N/A',
	@Browser VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @Count INT,
	@IsActive BIT,
	@IsDeleted BIT,
	@MustResetPassword BIT,
	@UID INT

SELECT @Count = isnull(Count(*), 0),
	@UID = UID,
	@IsActive = IsActive,
	@IsDeleted = IsDeleted
FROM dbo.SRPUser
WHERE Username = @UserName
	AND IsDeleted = 0
	AND IsActive = 1
GROUP BY UID,
	IsActive,
	IsDeleted

IF @Count = 0
	OR @Count IS NULL
BEGIN
	--SELECT
	--		*
	--FROM
	--	dbo.SRPUser
	--WHERE
	--	 Username is null
	SELECT 0
END
ELSE
BEGIN
	--SELECT
	--	*
	--FROM
	--	dbo.SRPUser
	--WHERE
	--	UID = @UID
	INSERT INTO dbo.SRPUserLoginHistory (
		UID,
		SessionsID,
		StartDateTime,
		IP,
		MachineName,
		Browser,
		EndDateTime
		)
	VALUES (
		@UID,
		@SessionId,
		getdate(),
		@IP,
		@MachineName,
		@Browser,
		NULL
		)

	--exec cbspSRPUser_GetAllPermissions @UID
	SELECT 1
END

RETURN 0
