
/****** Object:  StoredProcedure [dbo].[app_CodeType_GetByID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CodeType_GetByID] @CTID INT
AS
SELECT *
FROM [CodeType]
WHERE CTID = @CTID
