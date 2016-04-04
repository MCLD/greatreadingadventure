

CREATE PROCEDURE [dbo].[app_AvatarPart_Update] (
	@Name VARCHAR(50),
	@Gender VARCHAR(1),
	@ComponentID INT = 0,
	@BadgeID INT = 0,
	@Ordering INT = 0,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@TenID INT = 0,
	@APID INT OUTPUT
	)
AS
UPDATE AvatarPart
SET Name = @Name,
	Gender = @Gender,
	ComponentID = @ComponentID,
	BadgeID = @BadgeID,
	Ordering = @Ordering,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	TenID = @TenID
WHERE APID = @APID
	AND TenID = @TenID
