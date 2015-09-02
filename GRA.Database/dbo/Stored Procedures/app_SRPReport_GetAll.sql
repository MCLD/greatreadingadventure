
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SRPReport_GetAll] @TenID INT = NULL
AS
SELECT RID,
	ReportName,
	AddedDate
FROM [SRPReport]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
