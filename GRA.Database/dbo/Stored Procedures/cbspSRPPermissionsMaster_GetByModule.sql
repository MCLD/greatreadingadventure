
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_GetByModule]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_GetByModule] @ModID INT = - 1
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPPermissionsMaster
WHERE ModID = @ModID
