
CREATE FUNCTION [dbo].[fx_ConvertPoints] (
	@ProgID INT,
	@Points INT,
	@OutReadingType INT
	)
RETURNS DECIMAL(16, 4)
AS
BEGIN
	DECLARE @ret DECIMAL(16, 4)
	DECLARE @OutActivityTypePoints INT
	DECLARE @OutActivityTypeCount INT

	SELECT @OutActivityTypePoints = - 1,
		@OutActivityTypeCount = - 1

	SELECT @OutActivityTypePoints = PointCount,
		@OutActivityTypeCount = ActivityCount
	FROM ProgramGamePointConversion
	WHERE PGID = @ProgID
		AND ActivityTypeId = @OutReadingType

	IF (
			@OutActivityTypePoints IS NULL
			OR @OutActivityTypeCount IS NULL
			)
	BEGIN
		SET @ret = NULL -- -1.00
	END
	ELSE
	BEGIN
		IF (@OutActivityTypePoints = 0)
		BEGIN
			SET @ret = NULL -- -1.00
		END
		ELSE
		BEGIN
			SET @ret = convert(DECIMAL(16, 4), convert(DECIMAL(16, 4), @Points) * convert(DECIMAL(16, 4), @OutActivityTypeCount)) / convert(DECIMAL(16, 4), @OutActivityTypePoints)
		END
	END

	RETURN @ret
END
