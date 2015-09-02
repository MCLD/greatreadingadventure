
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_GetByID] @WMID INT
AS
SELECT *
FROM [MGWordMatch]
WHERE WMID = @WMID
