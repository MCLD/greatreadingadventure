
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Patron_GetByUsername] @Username VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM Patron
WHERE Username = @Username
