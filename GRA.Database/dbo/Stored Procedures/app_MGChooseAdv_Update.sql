
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdv_Update] (
	@CAID INT,
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGChooseAdv
SET MGID = @MGID,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE CAID = @CAID
