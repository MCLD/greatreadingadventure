
CREATE PROCEDURE [dbo].[app_Badge_Filter] @TenID INT = NULL,
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

	IF @BranchId IS NULL
	BEGIN
		SELECT b.*
		FROM [Badge] b
		WHERE (
				b.[TenID] = @TenID
				OR @TenID IS NULL
				)
			AND (
				(
					b.[AdminName] LIKE @SearchText
					OR b.[UserName] LIKE @SearchText
					OR b.[AddedUser] LIKE @SearchText
					)
				OR @SearchText IS NULL
				)
		ORDER BY b.[AdminName]
	END
	ELSE
	BEGIN
		SELECT b.*
		FROM [Badge] b
		LEFT OUTER JOIN [BadgeBranch] bb ON b.[BID] = bb.[BID]
		WHERE (
				b.[TenID] = @TenID
				OR @TenID IS NULL
				)
			AND (
				(
					b.[AdminName] LIKE @SearchText
					OR b.[UserName] LIKE @SearchText
					OR b.[AddedUser] LIKE @SearchText
					)
				OR @SearchText IS NULL
				)
			AND (
				bb.[CID] = @BranchID
				OR @BranchID IS NULL
				)
		ORDER BY b.[AdminName]
	END
END