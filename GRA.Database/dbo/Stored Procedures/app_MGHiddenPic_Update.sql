
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPic_Update] (
	@HPID INT,
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@EasyDictionary TEXT,
	@MediumDictionary TEXT,
	@HardDictionary TEXT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGHiddenPic
SET MGID = @MGID,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	EasyDictionary = @EasyDictionary,
	MediumDictionary = @MediumDictionary,
	HardDictionary = @HardDictionary,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE HPID = @HPID
