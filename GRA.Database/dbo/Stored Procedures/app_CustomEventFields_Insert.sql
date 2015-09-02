
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_CustomEventFields_Insert] (
	@Use1 BIT,
	@Label1 VARCHAR(50),
	@DDValues1 VARCHAR(50),
	@Use2 BIT,
	@Use3 BIT,
	@Label2 VARCHAR(50),
	@Label3 VARCHAR(50),
	@DDValues2 VARCHAR(50),
	@DDValues3 VARCHAR(50),
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
	@CID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO CustomEventFields (
		Use1,
		Label1,
		DDValues1,
		Use2,
		Use3,
		Label2,
		Label3,
		DDValues2,
		DDValues3,
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
		@Use1,
		@Label1,
		@DDValues1,
		@Use2,
		@Use3,
		@Label2,
		@Label3,
		@DDValues2,
		@DDValues3,
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

	SELECT @CID = SCOPE_IDENTITY()
END
