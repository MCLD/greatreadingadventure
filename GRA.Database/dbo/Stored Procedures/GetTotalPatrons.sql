
CREATE PROCEDURE [dbo].[GetTotalPatrons] (
	@startRowIndex INT = 0,
	@maximumRows INT = 0,
	@sortString VARCHAR(200) = 'p.PID desc',
	@searchFirstName VARCHAR(50) = '',
	@searchLastName VARCHAR(50) = '',
	@searchUsername VARCHAR(50) = '',
	@searchEmail VARCHAR(128) = '',
	@searchDOB DATETIME = NULL,
	@searchProgram INT = 0,
	@searchGender VARCHAR(2) = '',
	@TenID INT = NULL
	)
AS
DECLARE @SQL1 VARCHAR(8000)

IF LEN(@sortString) = 0
	SET @sortString = 'p.PID'

DECLARE @Filter VARCHAR(8000)

SELECT @Filter = ''

IF @searchFirstName <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' FirstName like ''%' + replace(@searchFirstName, '''', '''''') + '%'' '

IF @searchLastName <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' LastName like ''%' + replace(@searchLastName, '''', '''''') + '%'' '

IF @searchUsername <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' Username like ''%' + replace(@searchUsername, '''', '''''') + '%'' '

IF @searchEmail <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' EmailAddress like ''%' + replace(@searchEmail, '''', '''''') + '%'' '

IF @searchDOB IS NOT NULL
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' DOB = ''' + convert(VARCHAR, @searchDOB, 101) + ''' '

IF @searchDOB IS NOT NULL
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' DOB = ''' + convert(VARCHAR, @searchDOB, 101) + ''' '

IF @searchProgram <> 0
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' ProgID = ' + CONVERT(VARCHAR, @searchProgram) + ' '

IF @searchGender <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' Gender like ''%' + @searchGender + '%'' '

SELECT @Filter = @Filter + CASE len(@Filter)
		WHEN 0
			THEN ''
		ELSE ' AND '
		END + ' TenID = ' + convert(VARCHAR, @TenID) + ' '

SELECT @SQL1 = 'SELECT  count(*)
FROM Patron p ' + CASE len(@Filter)
		WHEN 0
			THEN ''
		ELSE ' WHERE ' + @Filter
		END

EXEC (@SQL1)
	--select @SQL1
