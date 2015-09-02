
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_Insert] (
	@CAID INT,
	@MGID INT,
	@Difficulty INT = 1,
	@StepNumber INT,
	@SlideText TEXT,
	@FirstImageGoToStep INT,
	@SecondImageGoToStep INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@CASID INT OUTPUT
	)
AS
BEGIN
	SELECT @StepNumber = isnull((
				SELECT isnull(MAX(StepNumber), 0)
				FROM MGChooseAdvSlides
				WHERE CAID = @CAID
					AND Difficulty = @Difficulty
				), 0) + 1

	INSERT INTO MGChooseAdvSlides (
		CAID,
		MGID,
		Difficulty,
		StepNumber,
		SlideText,
		FirstImageGoToStep,
		SecondImageGoToStep,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@CAID,
		@MGID,
		@Difficulty,
		@StepNumber,
		@SlideText,
		@FirstImageGoToStep,
		@SecondImageGoToStep,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @CASID = SCOPE_IDENTITY()
END
