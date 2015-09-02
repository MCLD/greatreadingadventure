
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_Update] (
	@PRCID INT,
	@PID INT,
	@BadgeID INT,
	@ProgID INT,
	@RewardCode VARCHAR(100),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE PatronRewardCodes
SET PID = @PID,
	BadgeID = @BadgeID,
	ProgID = @ProgID,
	RewardCode = @RewardCode,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE PRCID = @PRCID
