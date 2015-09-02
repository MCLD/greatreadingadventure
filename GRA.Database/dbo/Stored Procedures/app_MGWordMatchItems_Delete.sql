
/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGWordMatchItems_Delete] @WMIID INT
AS
DELETE
FROM [MGWordMatchItems]
WHERE WMIID = @WMIID
