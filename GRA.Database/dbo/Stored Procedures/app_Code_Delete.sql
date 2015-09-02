
/****** Object:  StoredProcedure [dbo].[app_Code_Delete]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Code_Delete] @CID INT
AS
DELETE
FROM [Code]
WHERE CID = @CID
