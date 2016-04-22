
CREATE PROCEDURE [dbo].[app_BookList_Filter] @TenID INT = NULL,
	@SearchText NVARCHAR(max) = NULL,
	@BranchId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ltrim(rtrim(@SearchText)) = ''
	BEGIN
		SET @SearchText = NULL
	END

	IF @BranchId = 0
	BEGIN
		SET @BranchId = NULL
	END

	SELECT bl.*,
		(
			SELECT COUNT(BLBID)
			FROM [BookListBooks] blb
			WHERE blb.[BLID] = bl.[BLID]
			) AS TotalTasks,
		ISNULL(p.[AdminName], '') AS [ProgName],
		ISNULL(c.[Code], '') AS [Library]
	FROM [BookList] bl
	LEFT JOIN [Code] c ON bl.[LibraryID] = c.[CID]
	LEFT JOIN [Programs] p ON bl.[TenID] = p.[TenID]
		AND bl.[ProgID] = p.[PID]
	WHERE (
			bl.[TenID] = @TenID
			OR @TenID IS NULL
			)
		AND (
			(
				bl.[ListName] LIKE @SearchText
				OR bl.[AdminName] LIKE @SearchText
				OR bl.[AddedUser] LIKE @SearchText
				)
			OR @SearchText IS NULL
			)
		AND (
			bl.[LibraryID] = @BranchID
			OR @BranchID IS NULL
			)
	ORDER BY bl.[AdminName]
END