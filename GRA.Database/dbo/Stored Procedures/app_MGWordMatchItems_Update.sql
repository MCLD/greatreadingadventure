
/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGWordMatchItems_Update] (
	@WMIID INT,
	@WMID INT,
	@MGID INT,
	@ItemImage VARCHAR(150),
	@EasyLabel VARCHAR(150),
	@MediumLabel VARCHAR(150),
	@HardLabel VARCHAR(150),
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGWordMatchItems
SET WMID = @WMID,
	MGID = @MGID,
	ItemImage = @ItemImage,
	EasyLabel = @EasyLabel,
	MediumLabel = @MediumLabel,
	HardLabel = @HardLabel,
	AudioEasy = @AudioEasy,
	AudioMedium = @AudioMedium,
	AudioHard = @AudioHard,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE WMIID = @WMIID
