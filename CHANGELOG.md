# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## [Unreleased]
### Added
- Better error handling in AJAX favoriting
- The ability to favorite a challenge from its detail page
- System to apply site-level settings throughout the application
- Site settings for hiding Events and Challenges in nav until registration opens
- Site setting for disabling secret code functionality
- Ability to add a small logo image in the page navigation
- Uploading files with markdown editor
- Mission Control ability to delete multiple history entries at once
- Site setting to restrict changing system and branch after signup
- Ability to create groups of challenges with unique URLs
- Ability to relate challenges and challenge groups to events

### Fixed
- Issue with favorites list filtering
- Issue with favorites spinner in Internet Explorer
- Problem with EF inferring an index that it shouldn't on UserFavoriteChallenges

### Removed
- "Is achiever" property from Users
- Application configuration value for hiding the secret code field

## [4.0.0-alpha8] 2017-12-04
### Added
- Questionnaire preview page
- Activity by Program Report
- Restrict household changes to when the site is open for registration or open
- Allow searching in Mission Control school lists
- Registrations and Achievers by School Report
- Award vendor code button
- Add prize redemption report
- Add participant prize report
- Add vendor code report
- System and branch configuration
- Dockerfile and .dockerignore to support Docker builds
- Categories for challenges
- Participants can now mark challenges as 'favorites'
- Markdown text entry and images to Challenge Tasks
- Participant Book list sorting
- Configuration for hiding secret code entry
- Note about password requirements to join form
- Allow "Events" to link to an external site rather than using built-in event system
- Garbage collection technical details to the system information page
- Option to link pages in the top-of-the-site navigation
- The ability to schedule free-form dashboard content

### Changed
- Show report completion percent in window titlebar
- Broadcast scheduling and sending
- Allow mails to be up to 2,000 characters long
- Anti-bounce on Mission Control school buttons
- Allow multiple programs to be selected for drawings
- Moved content into top-level shared folder for better Docker support
- Change rolling log configuration to support instances
- Travis CI automated build now uses Docker
- Challenge task sorting to now uses Ajax
- Use configured site-level SiteLogoUrl instead of the GRA logo
- Household and Mission control activity logging to now use Point Translation values
- User profile page header doesn't change based on what section the participant has selected

### Fixed
- Remove Entered School type restriction
- Issue where only one WebSocket could run at a time
- Show error message if initial WebSocket connection fails
- Do not show join links if the program is over
- Do not show Events or Challenges links if the program is over
- Configuration setting for database type (SQL Server or SQLite)
- Configuration setting for initial program setup (single program or multi-program)
- Allow importing avatars without any bundles
- Do not show avatar layer selector if only one layer
- Users without ViewUserPrizes coudln't view participant details
- Trim usernames so they can't have leading or trailing spaces
- Hide vendor code fields when site has no vendor codes created
- Fix case sensitivity issue with two reports

### Removed
- Remove static avatars; same functionality can be implemented with dynamic avatars

## [4.0.0-alpha7] 2017-07-03
### Added
- Exception handling when entering activity on the dashboard
- "Achieved at" property to Users
- Report: Badge Report
- Report: Badge Top Scores Report
- Report: Current Status By Program Report
- Report: Current Status Report
- Report: Participant Progress Report
- Report: Registrations and Achievers Report
- Report: Top Scores Report
- Mission Control interface for avatar bundles
- Vendor codes to user detail pages
- Ability to upload vendor status Excel (.xls) spreadsheets
- Event DateTime parsing error handling
- Drawing list filtering

### Changed
- Disable submit buttons on click for challenge and household pages
- Use WebSocket-based method for providing user feedback in long-running processes
- Mission Control user updating only updates certain properties
- Deleting a users history removes all that was awarded from it
- Identically entered schools will be processed the same way
- Order events by start date
- Better handling of adding badges/triggers to lists
- Trim user input on: card number, email, first name, last name, phone number, postal code, username
- "Register Member" to "Add Username" in Mission Control to match the public side
- Improve database indexes, remove unused/improper indexing
- Remove Mission Control participant username autofocus

### Fixed
- Issue where school CSV import fails if two schools have the same name
- Secret code GraException message typo
- Bundle award on login
- Daily image in IE
- Pressing enter submitting twice on household pages
- Disable task add/edit buttons on click (#290)
- Remove html from delete modal text (#201)
- Handling when adding items to a list from modal (#402)
- Household secret code entry permission errors
- Disable prize redeem/undo buttons on click
- Display of emoji usernames in MC profile page
- Vendor Codes being assigned twice
- Trigger permissions
- Drawing criteria timing out
- Entered Schools name trimming

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
