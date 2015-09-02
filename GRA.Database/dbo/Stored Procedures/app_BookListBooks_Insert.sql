
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_Insert]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_BookListBooks_Insert] (
	@BLID INT,
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@ISBN VARCHAR(50),
	@URL VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@BLBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO BookListBooks (
		BLID,
		Author,
		Title,
		ISBN,
		URL,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		TenID,
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
		@BLID,
		@Author,
		@Title,
		@ISBN,
		@URL,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@TenID,
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

	SELECT @BLBID = SCOPE_IDENTITY()
END
