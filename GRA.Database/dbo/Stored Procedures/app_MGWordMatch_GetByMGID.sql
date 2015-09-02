
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetByMGID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_GetByMGID] @MGID INT
AS
SELECT *
FROM [MGWordMatch]
WHERE MGID = @MGID
