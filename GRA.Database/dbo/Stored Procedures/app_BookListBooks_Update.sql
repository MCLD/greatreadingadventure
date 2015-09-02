
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_Update]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_BookListBooks_Update] (
	@BLBID INT,
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
	@FldText3 TEXT = ''
	)
AS
UPDATE BookListBooks
SET BLID = @BLID,
	Author = @Author,
	Title = @Title,
	ISBN = @ISBN,
	URL = @URL,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE BLBID = @BLBID
	AND TenID = @TenID
