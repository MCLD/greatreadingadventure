
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Patron_GetAll] @TenID INT = NULL
AS
SELECT p.*,
	pg.AdminName AS Program
FROM [Patron] p
LEFT JOIN Programs pg ON p.ProgID = pg.PID
WHERE (
		p.TenID = @TenID
		OR @TenID IS NULL
		)
