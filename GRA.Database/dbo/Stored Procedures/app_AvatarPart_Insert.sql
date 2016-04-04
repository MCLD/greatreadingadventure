
CREATE PROCEDURE [dbo].[app_AvatarPart_Insert] (
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
BEGIN
	INSERT INTO AvatarPart (
		Name,
		Gender,
	    ComponentID,
		BadgeID,
		Ordering,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		TenID
		)
	VALUES (
		@Name,
		@Gender,
		@ComponentID,
	    @BadgeID,
		@Ordering,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@TenID
		)

	SELECT @APID = SCOPE_IDENTITY()

	SELECT @APID
END
