
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_Insert]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_Insert] (
	@MAGID INT,
	@MGID INT,
	@Tile1UseMedium BIT,
	@Tile1UseHard BIT,
	@Tile2UseMedium BIT,
	@Tile2UseHard BIT,
	@Tile3UseMedium BIT,
	@Tile3UseHard BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@MAGTID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGMatchingGameTiles (
		MAGID,
		MGID,
		Tile1UseMedium,
		Tile1UseHard,
		Tile2UseMedium,
		Tile2UseHard,
		Tile3UseMedium,
		Tile3UseHard,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MAGID,
		@MGID,
		@Tile1UseMedium,
		@Tile1UseHard,
		@Tile2UseMedium,
		@Tile2UseHard,
		@Tile3UseMedium,
		@Tile3UseHard,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @MAGTID = SCOPE_IDENTITY()
END
