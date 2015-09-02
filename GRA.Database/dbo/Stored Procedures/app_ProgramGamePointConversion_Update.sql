
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_Update]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_Update] (
	@PGCID INT,
	@PGID INT,
	@ActivityTypeId INT,
	@ActivityCount INT,
	@PointCount INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE ProgramGamePointConversion
SET PGID = @PGID,
	ActivityTypeId = @ActivityTypeId,
	ActivityCount = @ActivityCount,
	PointCount = @PointCount,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE PGCID = @PGCID
