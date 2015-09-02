
/****** Object:  StoredProcedure [dbo].[app_ReportTemplate_Delete]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_ReportTemplate_Delete] @RTID INT
AS
DELETE
FROM [ReportTemplate]
WHERE RTID = @RTID
