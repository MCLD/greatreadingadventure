
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ReportTemplate_GetAll] @TenID INT = NULL
AS
SELECT RTID,
	ReportName
FROM [ReportTemplate]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
