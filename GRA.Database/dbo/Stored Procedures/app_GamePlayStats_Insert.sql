
/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_GamePlayStats_Insert] (
	@PID INT,
	@MGID INT,
	@MGType INT,
	@Started DATETIME,
	@Difficulty VARCHAR(50),
	@CompletedPlay BIT,
	@Completed DATETIME,
	@GPSID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO GamePlayStats (
		PID,
		MGID,
		MGType,
		Started,
		Difficulty,
		CompletedPlay,
		Completed
		)
	VALUES (
		@PID,
		@MGID,
		@MGType,
		@Started,
		@Difficulty,
		@CompletedPlay,
		@Completed
		)

	SELECT @GPSID = SCOPE_IDENTITY()
END
