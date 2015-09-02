
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ReportTemplate_GetByID] @RTID INT
AS
SELECT *
FROM [ReportTemplate]
WHERE RTID = @RTID
