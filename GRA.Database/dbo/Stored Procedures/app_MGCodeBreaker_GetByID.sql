
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_GetByID] @CBID INT
AS
SELECT *
FROM [MGCodeBreaker]
WHERE CBID = @CBID
