
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Offer_GetAll] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT AdminName
		FROM dbo.Programs p
		WHERE o.ProgramId = p.PID
		) AS Program,
	(
		SELECT Code
		FROM dbo.Code c
		WHERE o.BranchId = c.CID
		) AS Branch
FROM [Offer] o
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
