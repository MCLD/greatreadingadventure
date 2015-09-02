
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Update]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_Update] @PermissionID INT,
	@PermissionName VARCHAR(50),
	@PermissionDesc TEXT,
	@MODID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPPermissionsMaster
SET PermissionName = @PermissionName,
	PermissionDesc = @PermissionDesc,
	MODID = @MODID
WHERE PermissionID = @PermissionID
