# Permissions and Roles

The GRA software is managed in Mission Control. Access to Mission Control and the ability to add
content as well as change settings is provided via permissions and roles.

- A **permission** is an asserted right to see data or perform an action in the software.
- A **role** is a collection of permissions which can be granted to a user of the software.

## Permissions

These are the permissions available in the GRA:

- AccessFlightController - access to developer functions, for testing only - not to be given to users...here there be dragons
- AccessMissionControl - access to Mission Control, where all administration occurs
- AccessPerformerRegistration - access to Performer Scheduling administration in Mission Control
- ActivateAllChallenges - activate any challenge that is created
- ActivateSystemChallenges - activate challenges that are created in that user's system
- AddChallenges - add Challenges
- AddChallengeGroups - add Challenge Groups
- DeleteAnyMail - delete any Mail message in the software, even if it's not in their account
- DeleteParticipants - delete any Participant in the software
- EditChallenges - edit Challenges
- EditChallengeGroups - edit Challenge Groups
- EditParticipants - edit most information about a Participant but not their username
- EditParticipantUsernames - edit a Participant's username
- IgnorePointLimits - assign any amount of points to a Challenge or Trigger even if there's a limit set in the software
- ImportHouseholdMembers - import additional members into a Participant's household with an Excel file
- LogActivityForAny - log activity for any Participant in the program
- MailParticipants - send Mail to any Participant in the program from Mission Control
- ManageAvatars - import, create, delete, and make unlockable packages of Avatars
- ManageCarousels - manage Carousels of content to show on the Participant Dashboard
- ManageCategories - manage Categories and their colors (to be assigned to Challenges)
- ManageBulkEmails - manage and send emails to Participants who have registered with an email address and opted in
- ManageDailyLiteracyTips - manage and import Daily Literacy Tip images for prereader programs
- ManageDashboardContent - manage scheduled content to be displayed on the Participant Dashboard
- ManageEvents - manage Events
- ManageFeaturedChallengeGroups - manage Featured Challenge Groups: the ability to highlight Challenge Groups on the Challenges page
- ManageGroupTypes - manage types of groups - collections of participants that are larger than a family
- ManageLocations - manage physical locations referenced in Events and shown in the Participating Libraries view
- ManageNews - mange news postings in the Mission Control News Stand
- ManagePages - manage individual Pages of content (e.g. FAQ or Thanks content)
- ManagePointTranslations - _unused_ - will be for configuring how Participant entries translate to points earned
- ManagePerformers - manage Performer Scheduling sign-up and assignment to libraries
- ManagePrograms - manage Programs as listed in the software (e.g. Prereaders, Kids, Teens, Adults)
- ManageQuestionnaires - manage Questionnaires
- ManageRoles - manage Roles in the application
- ManageSchools - manage the list of Schools which can be requested upon sign-up
- ManageSites - manage Site configuration and settings
- ManageSocial - manage Social settings (e.g. OpenGraph)
- ManageSystems - manage Systems and Branches
- ManageTriggers - manage Triggers
- ManageTriggerMail - manage Trigger mail sending and content of the mails
- ManageVendorCodes - manage generation and administration of Vendor Codes
- NewsAutoSubscribe - set user to automatically receive emails about News Stand posts in Mission Control
- PerformDrawing - perform a Drawing based on a Drawing Criteria
- ReadAllMail - read all Mail in the software whether it is associated with their account or not
- ReceivePackingSlips - receive a packing slip and change the status of Vendor Code items based upon it
- RedeemBulkVendorCodes - can redeem Vendor Codes in bulk via upload
- RemoveChallenges - can remove Challenges from the software
- SchedulePerformers - is allowed to make selections in Performer Scheduling
- SendBroadcastMail - can send a mail to all (or a subset of) Participants
- SendBulkEmails - is permitted to send emails to all (or a subset) of Participants or subscribers
- TriggerAttachments - can add file attachments to Triggers (e.g. a certificate)
- UnDonateVendorCode - can undo a Participant's selection to donate their Vendor Code
- ViewAllChallenges - can view all Challenges in the software
- ViewAllReporting - can view all Reports in the software
- ViewGroupList - can view a list of all Groups in Mission Control
- ViewParticipantList - can view the list of all Participants
- ViewParticipantDetails - can see details about individual Participants
- ViewPerformerDetails - can view details of performers in Performer Scheduling
- ViewUnpublishedPages - can review Pages which have not yet been published
- ViewUserPrizes - can view Prizes associated with a Participant in Mission Control

## Roles

Roles are configured in Misson Control and allow the assignment of groups of permissions to Participants. The **System Administrator** role is a special role that has all permissions and cannot be deleted. This role should be utilized by a minimum number of staff and all staff accounts with it should have a strong password. In the roles area of Mission Control you can create new roles and add permissions to them.

Roles can be assigned to Participants manually from Mission Control or by an Authorization Code that Participants can enter when they Join the site or after joining by attempting to access Mission Control.
