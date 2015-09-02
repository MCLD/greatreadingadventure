
/****** Object:  StoredProcedure [dbo].[app_SRPReport_Delete]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SRPReport_Delete] @RID INT
AS
DELETE
FROM [SRPReport]
WHERE RID = @RID
