
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_Delete]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SentEmailLog_Delete] @EID INT
AS
DELETE
FROM [SentEmailLog]
WHERE EID = @EID
