
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_Update] (
	@CASID INT,
	@CAID INT,
	@MGID INT,
	@Difficulty INT,
	@StepNumber INT,
	@SlideText TEXT,
	@FirstImageGoToStep INT,
	@SecondImageGoToStep INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGChooseAdvSlides
SET CAID = @CAID,
	MGID = @MGID,
	Difficulty = @Difficulty,
	StepNumber = @StepNumber,
	SlideText = @SlideText,
	FirstImageGoToStep = @FirstImageGoToStep,
	SecondImageGoToStep = @SecondImageGoToStep,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE CASID = @CASID
