# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## [Unreleased]
### Added
- Add exception handling when entering activity on the dashboard
- Add achieved at property to Users
- Add WebSocket-based reporting interface to avoid timeouts on report execution

### Changed
- Change submit buttons to be disabled on click for challenge and household pages
- Change Mission Control user updating to only update certain properties
- Change deleting a users history to remove all that was awarded from it
- Change processing entered schools to perform the same action for identical entered schools
- Change events to order by start date

### Fixed
- Issue where school CSV import fails if two schools have the same name
- Fix secret code GraException message typo
- Fix bundle award on login
- Fix daily image in IE
- Fix pressing enter submitting twice on household pages
- Add max length checking when logging activity with books

## [4.0.0-alpha6] 2017-06-02
### Added
- Add disable and spinner to questionnaire submit buttons
- Add error message when trying to apply a role twice
- Add trigger checking on login and account creation
- Add names to program badges
- Add badges to questionnaires
- Add audit logging of removals
- Add branch validation to join step 1
- Add merging entered schools
- Add edit username to mission control
- Add household shortcut to navbar
- Add url to action tasks
- Add url hyperlink to challenge task list
- Add trigger mail permission
- Add showing original message when replying to mail in Mission Control
- Add mail permission requirement to drawing mails
- Add ability for users to edit books
- Add exclude previous winners drawing option
- GRA.CommandLine utilty for inserting test data
- Add daily program images
- Add page content area to dashboard
- Add unlocking avatar bundles
- Add authorization code validation
- Add cap to points earned
- Add ability to flush the sites from the cache in the flight controller
- Add exception handling when deleting a challenge task that a user has started

### Changed
- Change join branch list to be populated with all branches
- Change trigger requirement list styling
- Ability to browse community experiences from the events section
- Change events system dropdown to filter events
- Change recovery token log level to info
- Change MC book editing to match non-MC book editing
- Change challenge tasks to add books without points
- Remove reliance on content delivery networks
- Remove username when users deleted
- Remove secret code when trigger deleted
- Change trigger search to include secret code
- Disable page caching

### Fixed
- Add field to site object to allow forcing https even if the Web server believes the request came in via http.
- Fix missing hair colors
- Fix questionnaire controller not requiring authorization
- Fix jquery validation message not displaying on questionnaire
- Fix not being able to submit action tasks
- Fix participant sorting
- Fix joining and logging in while logged in
- Fix trigger badge creation exception
- Fix removing drawing winners
- Fix service broadcast mail permission
- Fix prize redemption exceptions not being caught
- Add event not found handling
- Fix dynamic avatar layer removability at extra small
- Fix dynamic avatar image paths
- Fix challenge task positioning
- Fix password reset users doesn't exist exception
- Fix UriBuilder exception handling
- Fix reporting overflow exception
- Fix login trigger points exception

## [4.0.0-alpha5] 2017-05-01
### Added
- Rudimentary ability to import events from a CSV file
- Rudimentary ability to import schools from a CSV file
- Added unobtrusive validation to user side
- Added ajax username/secret code available checking
- Added suppress notifications attribute
- Add disable and spinner to join submit buttons

### Changed
- Changed dynamic avatars
- Move profile, dashboard, and sign-up to be inside containers to make the site compatible with having a background color/image

### Fixed
- Fix alert spacing
- Fix signup empty program exception
- Fix signup validation messages
- Fix trigger creation exception
- Fix error logging in AuditingRepository - wasn't logging anything
- Improve appearance of assessments
- Fix In Use checking on empty fields

## [4.0.0-alpha4] 2017-04-14
### Added
- Added community experience event type
- Added end time/date to events

### Changed
- Changed trigger mail to require mail permissions
- Remove maximum length from question text
- Improve vendor code generation in FlightController

### Fixed
- Fix 'assets' directory to deploy properly with default avatars
- Fix MaxLength errors on user join and registration

## [4.0.0-alpha3] 2017-04-04
### Added
- Slack logging capability (see appsettings.json for configuration)
- Add adding participants from Mission Control
- Add filtering to MC participants list
- Add search and filtering to triggers and events lists
- Mission Control interface for managing dynamic avatars
- Add Mission Control questionnaire entry
- Add participant required questionnaire entry
- Add adding secret code when adding an events
- Add related events to trigger detail page
- Add system selection for event branch

### Changed
- Override Microsoft logging to limit severity to Error and higher
- Configure dynamic avatar parts in groups to make management easier
- Change Challenge Task Type selection to be panel-based

### Fixed
- Fix pagination for MC household add existing participant list
- Fix infinite recursion loop in triggers
- Fix displaying error for trying to delete a head of household

## [4.0.0-alpha2] - 2017-03-08
### Added
- Add database fields and front-end display for social.
- Ability to add an existing member to a household in Mission Control.
- System Information screen showing version numbers

### Changed
- Reformat trigger creation page in Mission Control.
- Upgraded to .NET Core Tools 1.0 and VS2017 project format.

### Fixed
- Fix household minute logging error when signed in as another user.
- Fix permission with sending mail for triggers.
- Fix issue with drop-down auto-selection in IE.
- Fix showing a response to a pre-open mail sign-up if no address is provided.
- Fix triggers so they can fire on less than all items

## [4.0.0-alpha1] - 2017-02-28
