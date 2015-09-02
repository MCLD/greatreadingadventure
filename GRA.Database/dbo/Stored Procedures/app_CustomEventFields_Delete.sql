
/****** Object:  StoredProcedure [dbo].[app_CustomEventFields_Delete]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_CustomEventFields_Delete] @TenID INT
AS
DELETE
FROM [CustomEventFields]
WHERE TenID = @TenID
