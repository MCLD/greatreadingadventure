
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_Get]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPGroups_Get] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPGroups
WHERE GID = @GID
