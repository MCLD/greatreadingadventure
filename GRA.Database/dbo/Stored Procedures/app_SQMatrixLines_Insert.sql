
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SQMatrixLines_Insert] (
	@QID INT,
	@LineOrder INT,
	@LineText VARCHAR(500),
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@SQMLID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SQMatrixLines (
		QID,
		LineOrder,
		LineText,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3
		)
	VALUES (
		@QID,
		(
			SELECT isnull(Max(LineOrder), 0) + 1
			FROM SQMatrixLines
			WHERE QID = @QID
			) -- @LineOrder
		,
		@LineText,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3
		)

	SELECT @SQMLID = SCOPE_IDENTITY()
END
