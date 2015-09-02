
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_Update] @UID INT,
	@Username VARCHAR(50),
	@Password VARCHAR(255),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@EmailAddress VARCHAR(128),
	@Division VARCHAR(50) = NULL,
	@Department VARCHAR(50) = NULL,
	@Title VARCHAR(50) = NULL,
	@IsActive BIT = 1,
	@MustResetPassword BIT = 0,
	@IsDeleted BIT = 0,
	@LastPasswordReset DATETIME = NULL,
	@ActionUsername VARCHAR(50),
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
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPUser
SET Username = @Username,
	Password = @Password,
	FirstName = @FirstName,
	LastName = @LastName,
	EmailAddress = @EmailAddress,
	Division = @Division,
	Department = @Department,
	Title = @Title,
	IsActive = @IsActive,
	MustResetPassword = @MustResetPassword,
	IsDeleted = @IsDeleted,
	LastPasswordReset = @LastPasswordReset,
	LastModDate = getdate(),
	LastModUser = @ActionUsername,
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
WHERE UID = @UID
	AND TenID = @TenID
