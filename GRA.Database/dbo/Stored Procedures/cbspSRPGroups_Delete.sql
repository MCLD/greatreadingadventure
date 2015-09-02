
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_Delete]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPGroups_Delete] @GID INT,
	@ActionUsername VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE
FROM dbo.SRPGroups
WHERE GID = @GID
