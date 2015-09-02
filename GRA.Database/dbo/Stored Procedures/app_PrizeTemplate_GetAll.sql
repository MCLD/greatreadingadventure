
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeTemplate_GetAll] @TenID INT
AS
SELECT t.*,
	ISNULL(p.AdminName, '') AS ProgName,
	ISNULL(c.Code, '') AS Library
FROM [PrizeTemplate] t
LEFT JOIN Programs p ON t.ProgID = p.PID
LEFT JOIN Code c ON t.PrimaryLibrary = c.CID
WHERE (
		t.TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY TID DESC
