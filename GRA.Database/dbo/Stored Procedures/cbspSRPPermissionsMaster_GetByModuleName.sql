
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_GetByModuleName]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_GetByModuleName] @ModuleName VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPPermissionsMaster.*
FROM dbo.SRPModule
INNER JOIN dbo.SRPPermissionsMaster ON dbo.SRPModule.ModId = dbo.SRPPermissionsMaster.MODID
WHERE dbo.SRPModule.ModName = @ModuleName
