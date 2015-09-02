
/****** Object:  StoredProcedure [dbo].[app_Notifications_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Notifications_Delete] @NID INT
AS
DELETE
FROM [Notifications]
WHERE NID = @NID
