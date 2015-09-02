
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_GetByID]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_GetByID] @PGCID INT
AS
SELECT *
FROM [ProgramGamePointConversion]
WHERE PGCID = @PGCID
