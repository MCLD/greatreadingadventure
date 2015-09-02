
/****** Object:  StoredProcedure [dbo].[app_Offer_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Offer_GetByID] @OID INT
AS
SELECT *
FROM [Offer]
WHERE OID = @OID
