
/****** Object:  StoredProcedure [dbo].[app_PatronBadges_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronBadges_Delete] @PBID INT
AS
DELETE
FROM [PatronBadges]
WHERE PBID = @PBID
