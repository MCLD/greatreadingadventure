
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetRandomX]    Script Date: 01/05/2015 14:43:22 ******/
CREATE PROCEDURE [dbo].[app_MGWordMatch_GetRandomX] @WMID INT,
	@Num INT = 3
AS
DECLARE @SQL VARCHAR(8000)

SELECT @SQL = 'select top ' + convert(VARCHAR, @Num) + ' NEWID() as id, * from  dbo.MGWordMatchItems Where WMID = ' + convert(VARCHAR, @WMID) + '  order by id'

EXEC (@SQL)
