
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Delete]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_Delete] @UID INT,
	@ActionUsername VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPUser
SET isDeleted = 1,
	DeletedDate = getdate(),
	LastModDate = getdate(),
	LastModUser = @ActionUsername
WHERE UID = @UID
