
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_GetByID]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SentEmailLog_GetByID] @EID INT
AS
SELECT *
FROM [SentEmailLog]
WHERE EID = @EID
