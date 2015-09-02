
/****** Object:  StoredProcedure [dbo].[app_Patron_GetByEmail]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_Patron_GetByEmail] @Email VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM Patron
WHERE EmailAddress = @Email
