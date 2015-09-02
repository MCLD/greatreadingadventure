
/****** Object:  StoredProcedure [dbo].[app_Event_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Event_Delete] @EID INT
AS
DELETE
FROM [Event]
WHERE EID = @EID
