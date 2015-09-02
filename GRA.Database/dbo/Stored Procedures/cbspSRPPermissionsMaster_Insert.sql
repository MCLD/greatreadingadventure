
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Insert]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_Insert] @PermissionID INT,
	@PermissionName VARCHAR(50),
	@PermissionDesc TEXT,
	@MODID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

INSERT INTO dbo.SRPPermissionsMaster (
	PermissionID,
	PermissionName,
	PermissionDesc,
	MODID
	)
VALUES (
	@PermissionID,
	@PermissionName,
	@PermissionDesc,
	@MODID
	)
