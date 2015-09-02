
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_Update] (
	@OBPGID INT,
	@OBID INT,
	@MGID INT,
	@PageNumber INT,
	@TextEasy TEXT,
	@TextMedium TEXT,
	@TextHard TEXT,
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGOnlineBookPages
SET OBID = @OBID,
	MGID = @MGID,
	PageNumber = @PageNumber,
	TextEasy = @TextEasy,
	TextMedium = @TextMedium,
	TextHard = @TextHard,
	AudioEasy = @AudioEasy,
	AudioMedium = @AudioMedium,
	AudioHard = @AudioHard,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE OBPGID = @OBPGID
