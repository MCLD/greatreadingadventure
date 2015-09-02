
/****** Object:  StoredProcedure [dbo].[app_Offer_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Offer_Delete] @OID INT
AS
DELETE
FROM [Offer]
WHERE OID = @OID
