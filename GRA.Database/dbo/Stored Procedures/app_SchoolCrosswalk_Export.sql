
CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_Export] @TenID INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT s.[Code] AS [SchoolName],
		st.[Code] AS [SchoolType],
		d.[Code] AS [DistrictName],
		NULLIF(scw.[MinGrade], 0) AS [MinGrade],
		NULLIF(scw.[MaxGrade], 0) AS [MaxGrade],
		NULLIF(scw.[MinAge], 0) AS [MinAge],
		NULLIF(scw.[MaxAge], 0) AS [MaxAge]
	FROM [schoolcrosswalk] scw
	INNER JOIN [code] s ON scw.[SchoolID] = s.[CID]
	INNER JOIN [code] st ON scw.[SchTypeID] = st.[CID]
	INNER JOIN [code] d ON scw.[DistrictID] = d.[CID]
	WHERE scw.[TenID] = @TenID
	ORDER BY d.[Code],
		st.[Code],
		s.[Code]
END