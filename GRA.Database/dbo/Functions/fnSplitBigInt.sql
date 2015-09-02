
CREATE FUNCTION [dbo].[fnSplitBigInt] (
	-- Add the parameters for the function here
	@input NVARCHAR(4000)
	)
RETURNS @retBigint TABLE ([Value] [bigint] NOT NULL)
AS
BEGIN
	DECLARE @bigint NVARCHAR(100)
	DECLARE @pos INT

	SET @input = LTRIM(RTRIM(@input)) + ',' -- TRIMMING THE BLANK SPACES
	SET @pos = CHARINDEX(',', @input, 1) -- OBTAINING THE STARTING POSITION OF COMMA IN THE GIVEN STRING

	IF REPLACE(@input, ',', '') <> '' -- CHECK IF THE STRING EXIST FOR US TO SPLIT
	BEGIN
		WHILE @pos > 0
		BEGIN
			SET @bigint = LTRIM(RTRIM(LEFT(@input, @pos - 1))) -- GET THE 1ST INT VALUE TO BE INSERTED

			IF @bigint <> ''
			BEGIN
				INSERT INTO @retBigint (Value)
				VALUES (CAST(@bigint AS BIGINT))
			END

			SET @input = RIGHT(@input, LEN(@input) - @pos) -- RESETTING THE INPUT STRING BY REMOVING THE INSERTED ONES
			SET @pos = CHARINDEX(',', @input, 1) -- OBTAINING THE STARTING POSITION OF COMMA IN THE RESETTED NEW STRING
		END
	END

	RETURN
END
