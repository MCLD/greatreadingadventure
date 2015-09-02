
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_DeleteAll]    Script Date: 01/05/2015 14:43:26 ******/
CREATE PROCEDURE [dbo].[app_SentEmailLog_DeleteAll] @EID INT
AS
DELETE
FROM [SentEmailLog]
