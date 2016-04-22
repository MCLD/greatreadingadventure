
CREATE PROCEDURE [dbo].[rpt_TenantStatusReport] @TenID INT,
	@BranchId INT = NULL,
	@DistrictId INT = NULL,
	@ProgramId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @RegisteredPatrons INT,
		@PointsEarned INT,
		@PointsEarnedReading INT,
		@ChallengesCompleted INT,
		@SecretCodesRedeemed INT,
		@AdventuresCompleted INT,
		@BadgesAwarded INT,
		@RedeemedProgramCodes INT
	DECLARE @Branches TABLE ([BranchId] INT)
	DECLARE @HasBranches BIT = 0

	IF @BranchId IS NULL
		AND @DistrictId IS NOT NULL
	BEGIN
		INSERT INTO @Branches
		SELECT [BranchId]
		FROM [LibraryCrosswalk]
		WHERE [DistrictID] = @DistrictId

		SET @HasBranches = 1
	END
	ELSE IF @BranchId IS NOT NULL
	BEGIN
		INSERT INTO @Branches
		VALUES (@BranchId)

		SET @HasBranches = 1
	END

	SELECT @RegisteredPatrons = count(PID)
	FROM [Patron] p
	WHERE p.[tenid] = @TenID
		AND (
			p.[ProgID] = @ProgramId
			OR @ProgramId IS NULL
			)
		AND (
			p.[PrimaryLibrary] IN (
				SELECT [BranchId]
				FROM @Branches
				)
			OR @HasBranches = 0
			)

	SELECT @PointsEarned = sum(pp.[NumPoints]),
		@PointsEarnedReading = sum(CASE pp.[AwardReasonCd]
				WHEN 0
					THEN 1
				ELSE 0
				END),
		@ChallengesCompleted = sum(CASE pp.[IsBookList]
				WHEN 1
					THEN 1
				ELSE 0
				END),
		@SecretCodesRedeemed = sum(CASE pp.[isEvent]
				WHEN 1
					THEN 1
				ELSE 0
				END),
		@AdventuresCompleted = sum(CASE pp.[AwardReasonCd]
				WHEN 4
					THEN 1
				ELSE 0
				END)
	FROM [patronpoints] pp
	INNER JOIN [patron] p ON p.[pid] = pp.[pid]
		AND p.[tenid] = @TenID
		AND (
			p.[ProgID] = @ProgramId
			OR @ProgramId IS NULL
			)
		AND (
			p.[PrimaryLibrary] IN (
				SELECT [BranchId]
				FROM @Branches
				)
			OR @HasBranches = 0
			)

	SELECT @BadgesAwarded = count(pbid)
	FROM [patronbadges] pb
	INNER JOIN [patron] p ON p.[pid] = pb.[pid]
		AND p.[tenid] = @TenID
		AND (
			p.[ProgID] = @ProgramId
			OR @ProgramId IS NULL
			)
		AND (
			p.[PrimaryLibrary] IN (
				SELECT [BranchId]
				FROM @Branches
				)
			OR @HasBranches = 0
			)

	SELECT @RedeemedProgramCodes = count(pc.[PCID])
	FROM [ProgramCodes] pc
	INNER JOIN [Patron] p ON p.[PID] = pc.[PatronId]
		AND p.[TenID] = @TenID
		AND (
			p.[ProgID] = @ProgramId
			OR @ProgramId IS NULL
			)
		AND (
			p.[PrimaryLibrary] IN (
				SELECT [BranchId]
				FROM @Branches
				)
			OR @HasBranches = 0
			)
	WHERE pc.[isUsed] = 1

	SELECT @RegisteredPatrons AS RegisteredPatrons,
		COALESCE(@PointsEarned, 0) AS PointsEarned,
		COALESCE(@PointsEarnedReading, 0) AS PointsEarnedReading,
		COALESCE(@ChallengesCompleted, 0) AS ChallengesCompleted,
		COALESCE(@SecretCodesRedeemed, 0) AS SecretCodesRedeemed,
		COALESCE(@AdventuresCompleted, 0) AS AdventuresCompleted,
		@BadgesAwarded AS BadgesAwarded,
		@RedeemedProgramCodes AS RedeemedProgramCodes
END