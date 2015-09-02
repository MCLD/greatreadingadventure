
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_Delete]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_Delete] @PGCID INT
AS
DELETE
FROM [ProgramGamePointConversion]
WHERE PGCID = @PGCID
