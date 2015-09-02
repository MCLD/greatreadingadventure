
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_Insert]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_Insert] (
	@PGID INT,
	@ActivityTypeId INT,
	@ActivityCount INT,
	@PointCount INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@PGCID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO ProgramGamePointConversion (
		PGID,
		ActivityTypeId,
		ActivityCount,
		PointCount,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@PGID,
		@ActivityTypeId,
		@ActivityCount,
		@PointCount,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @PGCID = SCOPE_IDENTITY()
END
