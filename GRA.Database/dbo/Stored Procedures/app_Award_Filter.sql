
CREATE PROCEDURE [dbo].[app_Award_Filter] @TenID INT = NULL,
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

	SELECT a.*,
		ISNULL(b.[AdminName], '') AS [BadgeName],
		ISNULL(cbranch.[Code], '') AS [Branch],
		ISNULL(p.[AdminName], '') AS [Program],
		ISNULL(cdistrict.[Code], '') AS [DistrictName],
		ISNULL(cschool.[Code], '') AS [SchName]
	FROM [Award] a
	LEFT JOIN [Badge] b ON b.[BID] = a.[BadgeID]
	LEFT JOIN [Code] cbranch ON cbranch.[CID] = a.[BranchID]
	LEFT JOIN [Programs] p ON p.[PID] = a.[ProgramID]
	LEFT JOIN [Code] cdistrict ON cdistrict.[CID] = a.[District]
	LEFT JOIN [Code] cschool ON cschool.[CID] = a.[SchoolName]
	WHERE (
			a.[TenID] = @TenID
			OR @TenID IS NULL
			)
		AND (
			(
				a.[AwardName] LIKE @SearchText
				OR a.[AddedUser] LIKE @SearchText
				)
			OR @SearchText IS NULL
			)
		AND (
			a.[BranchID] = @BranchID
			OR @BranchID IS NULL
			)
	ORDER BY a.[AwardName]
END