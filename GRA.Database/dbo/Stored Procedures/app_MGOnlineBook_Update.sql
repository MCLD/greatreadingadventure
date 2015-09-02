
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBook_Update] (
	@OBID INT,
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGOnlineBook
SET MGID = @MGID,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE OBID = @OBID
