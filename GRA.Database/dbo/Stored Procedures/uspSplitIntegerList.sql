
CREATE PROCEDURE [dbo].[uspSplitIntegerList] @list_integers TEXT
AS
SET NOCOUNT ON

DECLARE @InputLen INT -- input text length
DECLARE @TextPos INT -- current position within input text
DECLARE @Chunk VARCHAR(8000) -- chunk within input text
DECLARE @ChunkPos INT -- current position within chunk
DECLARE @DelimPos INT -- position of delimiter
DECLARE @ChunkLen INT -- chunk length
DECLARE @DelimLen INT -- delimiter length
DECLARE @Delimiter VARCHAR(3) -- delimiter
DECLARE @ItemBegPos INT -- item starting position in text
DECLARE @ItemOrder INT -- item order in list

-- ALTER table to hold list items
-- actually their positions because we may want to scrub this list eliminating bad entries before substring is applied
CREATE TABLE #list_items (
	item_order INT,
	item_begpos INT,
	item_endpos INT
	)

-- process list
IF @list_integers IS NOT NULL
BEGIN
	-- initialize
	-- notice that this loop assumes a delimiter length of 1
	-- if the delimiter is longer we have to deal with stuff like delimiters straddling the chunk boundaries
	SET @InputLen = DATALENGTH(@list_integers)
	SET @TextPos = 1
	SET @Delimiter = ','
	SET @DelimLen = DATALENGTH(@Delimiter)
	SET @ItemBegPos = 1
	SET @ItemOrder = 1
	SET @ChunkLen = 1

	-- cycle through input processing chunks
	WHILE @TextPos <= @InputLen
		AND @ChunkLen <> 0
	BEGIN
		-- get current chunk
		SET @Chunk = SUBSTRING(@list_integers, @TextPos, 8000)
		-- setup initial variable values
		SET @ChunkPos = 1
		SET @ChunkLen = DATALENGTH(@Chunk)
		SET @DelimPos = CHARINDEX(@Delimiter, @Chunk, @ChunkPos)

		-- loop over the chunk, until the last delimiter
		WHILE @ChunkPos <= @ChunkLen
			AND @DelimPos <> 0
		BEGIN
			-- insert position
			INSERT INTO #list_items (
				item_order,
				item_begpos,
				item_endpos
				)
			VALUES (
				@ItemOrder,
				@ItemBegPos,
				(@TextPos + @DelimPos - 1) - 1
				)

			-- adjust positions
			SET @ItemOrder = @ItemOrder + 1
			SET @ItemBegPos = (@TextPos + @DelimPos - 1) + @DelimLen
			SET @ChunkPos = @DelimPos + @DelimLen
			-- find next delimiter
			SET @DelimPos = CHARINDEX(@Delimiter, @Chunk, @ChunkPos)
		END

		-- adjust positions
		SET @TextPos = @TextPos + @ChunkLen
	END

	-- handle last item
	IF @ItemBegPos <= @InputLen
	BEGIN
		-- insert position
		INSERT INTO #list_items (
			item_order,
			item_begpos,
			item_endpos
			)
		VALUES (
			@ItemOrder,
			@ItemBegPos,
			@InputLen
			)
	END

	-- delete the bad items
	DELETE
	FROM #list_items
	WHERE item_endpos < item_begpos

	-- return list items
	SELECT CAST(SUBSTRING(@list_integers, item_begpos, (item_endpos - item_begpos + 1)) AS INT) AS item_integer,
		item_order,
		item_begpos,
		item_endpos
	FROM #list_items
	WHERE ISNUMERIC(SUBSTRING(@list_integers, item_begpos, (item_endpos - item_begpos + 1))) = 1
	ORDER BY item_order
END

DROP TABLE #list_items

RETURN
