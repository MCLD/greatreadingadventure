
CREATE PROCEDURE [dbo].[app_Event_InitTenant] @src INT,
	@dst INT
AS
INSERT INTO Event (
	EventTitle,
	EventDate,
	EventTime,
	HTML,
	SecretCode,
	NumberPoints,
	BadgeID,
	BranchID,
	Custom1,
	Custom2,
	Custom3,
	LastModDate,
	LastModUser,
	AddedDate,
	AddedUser,
	TenID,
	FldInt1,
	FldInt2
	--,FldInt3
	,
	FldBit1,
	FldBit2,
	FldBit3,
	FldText1,
	FldText2,
	FldText3,
	EndDate,
	EndTime,
	ShortDescription,
	FldInt3
	)
OUTPUT 'event',
	@dst,
	[inserted].FldInt3,
	GETDATE(),
	[inserted].[EID]
INTO TenantInitData(IntitType, DestTID, SrcPK, DateCreated, DstPK)
SELECT e.EventTitle,
	e.EventDate,
	e.EventTime,
	e.HTML,
	e.SecretCode,
	e.NumberPoints,
	0 -- CANT DO BADGE
	,
	0 -- CANT DO BRANCH
	,
	e.Custom1,
	e.Custom2,
	e.Custom3,
	e.LastModDate,
	e.LastModUser,
	e.AddedDate,
	'SYSADMIN',
	@dst,
	e.FldInt1,
	e.FldInt2
	--,e.FldInt3
	,
	e.FldBit1,
	e.FldBit2,
	e.FldBit3,
	e.FldText1,
	e.FldText2,
	e.FldText3,
	e.EndDate,
	e.EndTime,
	e.ShortDescription,
	e.EID
FROM Event e
WHERE e.TenID = @src
	AND e.EID NOT IN (
		SELECT SrcPK
		FROM TenantInitData
		WHERE IntitType = 'event'
			AND DestTID = @dst
		)
