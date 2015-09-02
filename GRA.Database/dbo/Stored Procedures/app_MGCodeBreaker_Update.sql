
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_Update] (
	@CBID INT,
	@MGID INT,
	@EasyString VARCHAR(250),
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@MediumString VARCHAR(250),
	@HardString VARCHAR(250),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGCodeBreaker
SET MGID = @MGID,
	EasyString = @EasyString,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	MediumString = @MediumString,
	HardString = @HardString,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE CBID = @CBID
