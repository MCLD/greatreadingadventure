
/****** Object:  StoredProcedure [dbo].[app_CodeType_GetAll]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CodeType_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [CodeType]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
