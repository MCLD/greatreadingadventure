
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_GetByPage] @Page INT,
	@OBID INT
AS
SELECT *
FROM [MGOnlineBookPages]
WHERE PageNumber = @Page
	AND OBID = @OBID
