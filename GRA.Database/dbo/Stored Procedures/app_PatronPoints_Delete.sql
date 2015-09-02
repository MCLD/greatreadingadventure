
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_Delete] @PPID INT
AS
DELETE
FROM [PatronPoints]
WHERE PPID = @PPID
