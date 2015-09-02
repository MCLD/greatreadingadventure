
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_GetByName]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_GetByName] @PermissionName VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPPermissionsMaster
WHERE PermissionName = @PermissionName
