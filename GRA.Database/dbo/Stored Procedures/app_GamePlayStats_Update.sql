
/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_GamePlayStats_Update] (
	@GPSID INT,
	@PID INT,
	@MGID INT,
	@MGType INT,
	@Started DATETIME,
	@Difficulty VARCHAR(50),
	@CompletedPlay BIT,
	@Completed DATETIME
	)
AS
UPDATE GamePlayStats
SET PID = @PID,
	MGID = @MGID,
	MGType = @MGType,
	Started = @Started,
	Difficulty = @Difficulty,
	CompletedPlay = @CompletedPlay,
	Completed = @Completed
WHERE GPSID = @GPSID
