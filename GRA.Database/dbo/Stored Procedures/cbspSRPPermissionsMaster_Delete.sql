
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Delete]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_Delete] @PermissionID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE
FROM dbo.SRPPermissionsMaster
WHERE @PermissionID = PermissionID
