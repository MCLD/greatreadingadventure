
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_Update] (
	@MAGTID INT,
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
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGMatchingGameTiles
SET MAGID = @MAGID,
	MGID = @MGID,
	Tile1UseMedium = @Tile1UseMedium,
	Tile1UseHard = @Tile1UseHard,
	Tile2UseMedium = @Tile2UseMedium,
	Tile2UseHard = @Tile2UseHard,
	Tile3UseMedium = @Tile3UseMedium,
	Tile3UseHard = @Tile3UseHard,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE MAGTID = @MAGTID
