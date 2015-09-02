
CREATE FUNCTION [dbo].[fnSplitString] (
	-- Add the parameters for the function here
	@input NVARCHAR(4000),
	@CharSplit VARCHAR(1) = ','
	)
RETURNS @retString TABLE ([Value] VARCHAR(255) NOT NULL)
AS
BEGIN
	DECLARE @string NVARCHAR(255)
	DECLARE @pos INT

	SET @input = LTRIM(RTRIM(@input)) + @CharSplit -- TRIMMING THE BLANK SPACES
	SET @pos = CHARINDEX(@CharSplit, @input, 1) -- OBTAINING THE STARTING POSITION OF COMMA IN THE GIVEN STRING

	IF REPLACE(@input, @CharSplit, '') <> '' -- CHECK IF THE STRING EXIST FOR US TO SPLIT
	BEGIN
		WHILE @pos > 0
		BEGIN
			SET @string = LTRIM(RTRIM(LEFT(@input, @pos - 1))) -- GET THE 1ST INT VALUE TO BE INSERTED

			IF @string <> ''
			BEGIN
				INSERT INTO @retString (Value)
				VALUES (@string)
			END

			SET @input = RIGHT(@input, LEN(@input) - @pos) -- RESETTING THE INPUT STRING BY REMOVING THE INSERTED ONES
			SET @pos = CHARINDEX(@CharSplit, @input, 1) -- OBTAINING THE STARTING POSITION OF COMMA IN THE RESETTED NEW STRING
		END
	END

	RETURN
END
