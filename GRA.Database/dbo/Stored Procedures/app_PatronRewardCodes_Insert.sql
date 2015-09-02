
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_Insert] (
	@PID INT,
	@BadgeID INT,
	@ProgID INT,
	@RewardCode VARCHAR(100),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@PRCID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronRewardCodes (
		PID,
		BadgeID,
		ProgID,
		RewardCode,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@PID,
		@BadgeID,
		@ProgID,
		@RewardCode,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @PRCID = SCOPE_IDENTITY()
END
