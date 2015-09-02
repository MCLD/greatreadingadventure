
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_Update] (
	@HPBID INT,
	@HPID INT,
	@MGID INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGHiddenPicBk
SET HPID = @HPID,
	MGID = @MGID,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE HPBID = @HPBID
