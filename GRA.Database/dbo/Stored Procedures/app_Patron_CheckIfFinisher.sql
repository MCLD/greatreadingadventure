
CREATE PROCEDURE [dbo].[app_Patron_CheckIfFinisher] @PID INT = NULL
AS
SELECT isnull(dbo.fx_IsFinisher(p.PID, p.ProgID), 0) AS IsFinisher
FROM Patron p
WHERE p.PID = @PID
