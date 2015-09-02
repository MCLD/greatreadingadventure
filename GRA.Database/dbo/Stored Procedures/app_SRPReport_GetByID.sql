
/****** Object:  StoredProcedure [dbo].[app_SRPReport_GetByID]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SRPReport_GetByID] @RID INT
AS
SELECT *
FROM [SRPReport]
WHERE RID = @RID
