
CREATE FUNCTION [dbo].[fx_IsFinisher] (
	@PID INT,
	@ProgID INT
	)
RETURNS BIT
AS
BEGIN
	DECLARE @ret BIT
	DECLARE @GameCompletionPoints INT
	DECLARE @UserPoints INT

	IF (
			@PID IS NULL
			OR @ProgID IS NULL
			OR @ProgID = 0
			)
	BEGIN
		SET @ret = 0
	END
	ELSE
	BEGIN
		SELECT @GameCompletionPoints = IsNull(CompletionPoints, 0)
		FROM Programs
		WHERE PID = @ProgID

		/*
		if (select ProgramGameID from Programs where PID = @ProgID) = 0 
		begin
			select @GameCompletionPoints = IsNull(CompletionPoints,0) from Programs where PID = @ProgID
		end
		else
		begin

			select @GameCompletionPoints = isnull(SUM(isnull(pgl.PointNumber,0)),0)
			from ProgramGame pg 
					left join ProgramGameLevel pgl
						on pg.PGID = pgl.PGID
					left join Programs p
						on p.ProgramGameID = pg.PGID
			where 
				p.PID = @ProgID
		end
		*/
		SELECT @UserPoints = isnull(SUM(isnull(NumPoints, 0)), 0)
		FROM PatronPoints
		WHERE PID = @PID

		SELECT @ret = CASE 
				WHEN @UserPoints < @GameCompletionPoints
					OR @GameCompletionPoints = 0
					THEN 0
				ELSE 1
				END
	END

	RETURN @ret
END
