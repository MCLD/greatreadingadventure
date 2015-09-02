
/****** Object:  StoredProcedure [dbo].[app_SRPSettings_GetByID]    Script Date: 01/05/2015 14:43:27 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_GetByID] @SID INT
AS
SELECT *
FROM [SRPSettings]
WHERE SID = @SID
