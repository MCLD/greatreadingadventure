# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## [Unreleased]

## Added

- Alt texts to avatar color, item and layer selector images
- Mission Control interface to set avatar color and item alt texts
- BranchId token to vendor code URLs
- Setting performer programs to be unapproved
- Performer field for contact name
- Performer scheduling settings for cover sheet fields
- View of and access to historical report output
- QR code generation ability
- Spanish localization for jQuery validation messages
- News Post pinning
- Email address validation
- Cannot be emailed property to users
- Participant personal point goal
- Join codes for tracking signups
- Ability to override activity button text
- Pruning inactive users
- Staff login report
- Info message when editing Point Translations with unchangeable properties

## Changed

- Update packages
- Update Docker lock checksums
- Performer references to be a text input instead of file
- Update front-end packages
- Add achiever column to badge report
- Drawing mail to be sent manually after a drawing is performed
- Vendor status message to include branch name when ready for pick-up

## Fixed

- Display of modals after Bootstrap 5.3 upgrade
- Performer program cost to display in currency format
- Email reminder signup source being editable in html
- Missing translations for secret codes, books and events
- Password change confirmation not functioning
- Challenge category multiselect formatting for participants
- Join issues caused by setting a program or branch with an authorization code
- Secret code field showing when it shouldn't in Mission Control activity logging

## [4.6.0] 2024-10-09

## Added

- "Ordered not Shipped" vendor report
- "Shipped not Arrived" vendor report
- "Titles not shipped" vendor report
- Ability to restrict report access by permission
- Clickable hyperlinks to Web-view on reports
- Avatar import duration to success pop-up
- Ability to restrict challenges to activate on more than a configured amount of points
- Trigger filter based on a configured "low point" threshhold
- Autocomplete properties to join and sign-in forms
- Welcome message site setting at top of join pages
- Editing of text segments
- "AddParticipant" permission for showing "Add Participant" button in Mission Control
- Drawing winner spreadsheet download
- Report criteria shows for selected badges/challenges/triggers on Excel metadata sheet
- Hints about how many daily images are needed on daily image upload page
- Handling of re-shipped vendor items when an imported order date is after a recorded ship date
- Mission Control interface for managing daily images

## Changed

- Update UI to support Bootstrap 5.3
- Improve pattern for selecting system and/or branch report criteria
- Better vendor code status overview
- Add tracking of packing list viewing
- Move exit and landing page messages to database
- Remove references to ValueTuple
- Replace AutoMapper with Mapster

### Fixed

- Update packages
- Update Docker Lock checksums
- Update pinned curl version
- Resolved issue with performer back-to-back dropdown on failed submit
- Improve performer selection view in MC
- Culture links to work regardless of case
- Fix "points" to be singular if only one point awarded
- Workaround for broken URL entry in Challenge editor in modal
- Issue preventing deletion of locations without associated events
- Issue where cloning a Community Experience would create an event
- Issue where a failed save of a Community Experience would change it to an event
- Initial branch selection on join to only autoselect if there is a single configured branch
- Fix branch deletion to reassign deleted users as well as active users
- Formatting issues with revised packing slip
- Error when code is assigned prior to one being earned
- Display of "Add Participant" button in Mission Control to be related to showing the group selector
- Handle data types when outputting spreadsheet from JSON (i.e. from reporting)
- Require sheet names for Excel exports to eliminate errors
- Sequences of numbers in Excel export no longer show green tab in cells
- Issue with drawing table of drawing winners
- Navigation selection on the top of the Badge Top Scores report criteria form
- Page preview links in Mission Control
- Issue with proper pagination while searching to add household member in MC
- Issue with filtering of deleted participants while searching to add household member in MC
- Replace string interpolation in logging statements

## [4.5.0] 2024-08-29

## Changed

- Update to ASP.NET Core 8.0

## [4.4.2] 2024-08-28

## Added

- Coversheet generation for performer invoices
- System setting for performer livestreaming question
- System setting for performer liability insurance question
- System setting to configure acceptable back-to-back intervals
- Last update information on performer scheduling selections
- Require day of event contact information when scheduling performer
- End date to streaming program list on dashboard and streaming page
- Filters applied to challenge list are retained in session cookie
- Scheduled task execution capability
- Welcome email capability
- Streamlined sign up options with authorization codes
- Site setting to prompt new registrants to add a family member
- 1,500 and 2,000 point values to the Participant Progress Report
- Age group and group name to vendor-generated hold slips
- Spreadsheet export of scheduled performers
- Configuraton for amount of welcome emails to send per job run
- Capability of handling a situation where a participant has multiple primary vendor codes assigned
- Participant filter for those who have multiple primary vendor codes assigned
- Configuration setting "GraSuppressTextLogs" to suppress writing text file logs

## Changed

- Improved release prep and publish scripts
- Back-to-back interval now selectable from a list
- Overall goal to "Community Reading Goal" so it works for all seasons
- Changed display of "Program" on the participant side to "Age Group"
- Book title and author now display in user history if provided
- Allow hold slip generation without marking vendor packing slip as received

## Fixed

- At a glance report now works with SQLite
- Trim coupon codes when they come in from a status update spreadsheet
- Bug in Remaining Vendor Prize report - deleted users with prizes caused error
- Remove references to deprecated GetBaseUrl call
- Automatically direct performers to the performer area if they sign in before the program opens
- Issue with selecting Drawing Criteria filtered by program
- Restrict ability to assign two primary vendor codes to a user via trigger
- Bug with scrolling some avatar layers at the mobile responsive level
- Move cache busting on trigger edit to ViewModel due to occsaional object output weirdness

## [4.4.1] 2023-08-11

## Changed

- Rename "Redemption Instructions" label to "Redemptions Instructions for Staff"
- Update to use avatar package 4.2.2
- Improve Google Analytics to better support GA 4 and events
- Order packing slip and hold slip views by detail, followed by last name and first name
- Fix display of packing slip to show the name as last name - comma - first name
- Upgrade from ASP.NET v7.0 to v7.0.10

## Added

- Notes field on prize redemption for staff
- Ability to change logging level from Seq using a logging level switch
- Docker source container file pinning
- Ability to reassign user branch when a branch is deleted
- Links to performers, programs, and kits in performer scheduling
- Allow everyone with scheduling permissions to see references
- Site-wide program reading goal
- Ability to reassign a participant a new vendor code
- Ability to assign self new auxiliary vendor codes
- Participant names to packing slip detail
- Ability to print hold slips from packing slip detail
- Id values to system/branch export
- Individual news post viewing in Mission Control optimized for printing
- Post count next to category names on Mission Control dashboard
- Ability to view history of changes submitted by another user from MC
- Internationalization for Vendor Code communications
- Prize view for participants (excluding drawing prizes)
- System setting to disable badge maker
- System setting to configure background color
- Downloadable ZIP files of badge images for each system from Mission Control
- Ability to attach, remove, or update a certificate (uploaded PDF) to a trigger
- News stand post updates are shown as updated and sorted by update date
- Uploading a list of vendor codes to invalidate them and send participants new ones
- Better error handling for type conversion errors on vendor code status report import
- Reassigned vendor codes can now be used in participant search

### Fixed

- Display a friendly and accurate error if a social image is too large
- Properly return appropriate HTTP status codes on page errors
- Logging when the ErrorController is hit is no longer excluded from most log sources
- Exception when trying to add a Role with a blank name
- Decouple one-to-one mapping of vendor codes to participants
- Branch/system import if the spreadsheet does not contain IDs
- Clear cached unread mail count upon authentication
- Updated URL to badge maker
- Move zoom button for avatars in mobile mode to top so it's always tappable
- Proper display of authorization code usage on edit modal
- Issue with "login as" on the profile list of associated participants
- Broken link from events in Mission Control to associated challenges
- Proper rendering of vendor code (templated) messages
- Mail unsubscribe links
- Reassigned codes no longer say the user is deleted in packing slip summaries

## [4.4.0] 2022-11-29

### Changed

- Upgrade from ASP.NET v6.0 to v7.0

## [4.3.17] 2022-11-17

### Added

- Notation on the participant dashboard that title and author are optional
- Site setting to disable avatar sharing
- Performer program setup supplemental text
- Remaining vendor prize report
- Vendor code by program report
- Challenge sorting options (most recent, most popular)
- Packing slip functionality for checking item status on arrival
- Sign in again button on exit pages when the program is running
- Site setting to show secret codes on the streaming event pages
- Ability to configure minimum worker threads and completion port threads
- Featured challenge group scheduling and display carousel
- Show reasons on MC participant history page as to why items cannot be deleted
- Ability to mark vendor items as not damaged and not missing in case of accidental box checking
- Ability to disable the in-software mail capability
- Report to see a count of pending vendor code items per branch
- Program-wide alert to dashboard page

### Changed

- Hide avatar bundle icon if no bundles are available
- Reduce streaming video caching on dashboard to 15 minutes
- Order streaming videos on dashboard by start date descending (newest at the top)
- Vendor item status display on participants profile
- Show wrench menu to all who have Mission Control access
- Upgrade from ASP.NET v5.0 to v6.0
- Moved TempData storage to session from separate cookie
- Updated Remaining Vendor Prize Pick-Up to separate out first/last names and username
- Import of avatar file will overwrite any files in teh avatar directories

### Fixed

- Fix location of uploaded avatar file
- Fix #816 Incorrect Profile tooltip annotations referenced
- Fix landing page translation and add culture in date formatting
- Preemptive button spinner on performer registration preventing submission
- Household prize redemption directing to the wrong controller
- Performer view selections button being visible for non-approved performers
- Fix bug binding performer archiving and streaming choices
- Hide branch selection in performer scheduling when no branches are available
- Mission Control error if a user who posted a news post was deleted
- Show friendly message when invalid image type is uploaded
- Unify email sending to use a single process with logging
- Allow Mission Control editing of email templates
- Add email internationalization
- Show error if user tries to log in as another user that they cannot
- Log error if initial user email check fails
- Show friendly error when geolocation fails
- Bug with avatar translated text display error
- Check for null viewmodel on some posts
- Bug with empty username in UsernameInUseAsync
- Allow deletion of events even if participants have favorited them
- Expiration of social card cache
- Site setting categories in Mission Control are now in alphabetical order

### Updated

- Package references

### Removed

- GRA.CommandLine project

## [4.2.1] - 2021-10-14

### Added

- Add accessibility (alt tags) to badges
- Import and export of Systems and Branches
- Import and export of Schools and Districts
- Mission Control interface to add daily images
- Analytics tracking of clicks on daily images
- Mission Control interface to manage social images
- Internationalization of social card images
- Add social cards which can be scheduled
- Ability to see a list of users in each role
- Administration and management of vendor codes from Mission Control
- Packing slip functionality for showing when vendor code awards have arrived
- Fix #741 Mission Control interface to add daily tips
- Fix #354 Add click tracking on daily image
- Unified approach to spinner buttons to debounce submissions

### Changed

- Extended length of stremaing program links to 2000 characters
- Improve accessibility throughout the application
- Move to updated FontAwesome glyph references

### Fixed

- Add accessibility to JavaScript modals throughout the application
- News posts now send emails via the job system to avoid timeouts
- Avatar imports now work if the avatars aren't in the root of the ZIP file
- Name all cookies with the discriminator value to avoid cookie collisions on the same host
- Visual issue with close button (X) on pop-up modals
- Mission Control household add existing participant list being empty
- Streaming event missing badge alt-text field
- News posts now include the subject correctly
- Event spatial proximity searching
- Household secret code entry awarding mail/prize to the user applying the codes
- Proper handling of the "Add Book" button prior to program start
- Allow special characters in authorization codes
- Extend length of streaming link
- Error on exit page is user/branch can't be determined
- Fix remove button logic for handling avatar body layer
- Fix #784 not-allowed cursor on inputs in FireFox
- Fix #619 Add date to "created by" display in Mission Control
- Fix #460 Ability to link to participant's library in the footer

## [4.2.0] - 2021-01-04

### Added

- Ping analytics upon account login
- Improve analytics for sign-ups to include if it's a first-time participant (fix #330)
- Ability to add carousel to dashboard
- System to facilitate performer registration and scheduling
- Ability to ask participants if they want to subscribe to emails during sign up
- News posts in Mission Control
- Automatic updating Mission Control at-a-glance report and unread mail status and count
- Redirect administrative users to Mission Control if the program isn't currently open
- Container labels following opencontainers.org annotation specification
- More information in Mission Control system information
- Mission Control menu item for entering an Authorization Code
- Improve Event and Community Experience discoverability with spatial proximity searching
  (requires Google Maps API key)
- Ability to configure a maximum activity amount in Site Settings
- Dashboard alert for pending vendor code redemption
- Default "System Account" for system-created items
- Show points earned in participant list
- Report for tracking prize redemptions
- Ability for participants to remove household members
- Household bulk prize redemption
- Group import from Excel spreadsheets
- Group lookup
- Avatar update google analytics event
- Language selection drop-down
- Spanish language translations of join, sign in, and the dashboard
- Email unsubscribe with token
- Checking for user join badges on sign in
- Avatar unavailable items list
- Daily image size setting
- Landing page preview ability (/Home/Preview/StageName)
- Show "created by" in MC on Challenges, Triggers, Events, Community Experiences, Drawings
- Disable and spinner to Challenge Task delete button
- Disable and spinner to sign in submit buttons
- Mission control button to update user triggers
- Redeem all button for household vendor codes
- Participant age to Group Vendor Code Report
- Add news auto subscribe permission
- Look up participants by vendor code
- Creation and sending of emails to subscribed participants from Mission Control
- Show associated system and branch on prize redemption screen
- Display participant system and branch in Mission Control mail view
- Vendor code generation through WebSocket job
- Site setting to hide events until program open
- Email award option for vendor codes
- User favoriting of events
- User filtering challenges and events by completed/uncompleted
- User badge gallery
- Badge max file size site setting
- Rudimentary health check

### Changed

- Script docker-build.bash to only build release images when branch starts with 'release/'
- From Google analytics.js to tag manager for analytics
- Location of favicons from wwwroot to shared/content so they can be easily customized
- Password recovery misses and reset errors are now log level info rather than warn
- Persist the data protection key to the database
- Improve school selection
- 'Limit to program' in Challenges is now 'associate with program'
- Participant progress bar to default to program goal is no personal reading goal is set
- Household member list to be sorted alphabetically
- Household page layout
- Upgrade Automapper from 6.0.2 to 8.0.0
- Add assemblies to AutoMapper service injection
- Household import to use the job system
- Reporting SQL IN clause error
- Badge image dimension restrictions
- Upgrade from ASP.NET Core v2.2 to v5.0

### Fixed

- Instance name enrichment for logging
- Error if account disappears while user is logged in
- Avatar options now showing with single layer
- Category color being unset on edit
- Display of Bootstrap drop-downs
- Permissions for insert sample data call
- Admin role permission updating using client-side evaluation
- Edit Point Translation in Mission Control so that it saves properly
- Bug with deleting a program with associated deleted users
- Startup log message about PsProgram Cost column precision
- Category empty description null reference exception
- Viewing daily images on the household page
- Password recovery validation error handling
- Challenge task type not being set correctly
- Badge requirements list using client-side evaluation
- Community experience report using client-side evaluation
- Return 404s for missing Mission Control files
- Layer item selector not changing the initial slide when an items selected
- Avatar items using the wrong layers initial color
- Un-donate buttons using the wrong permission
- Add book erroring when author field is empty
- Viewing participants with schools without the edit permission
- Profile school values disappearing after model error

### Removed

- Recovery error logging at the controller level - it's logged in the
  service
- SQL Server 2008 support

## [4.1.1] - 2018-12-13

### Added

- Add site.js to shared folder for customizing site scripting

### Changed

- New mail icon in navigation is red and solid for new messages, else grey and wireframe
- Upgrade ImageSharp from v1.0.0-beta0003 to v1.0.0-beta0005
- Move site.css for customizing site styles to shared folder
- Only redirect to session expired if participant has authenticated in last 2 hours
- Upgrade from ASP.NET Core v2.1 to v2.2

### Fixed

- Fix #377 Adding a challenge task causes an error
- Fix #382 Adding a drawing criterion fails if no or all programs are selected
- Fix #383 Viewing a challenge group causes a timeout
- Fix #379 Stacked Font Awesome icons are not working properly
- Fix #380 Group button showing on participant household with groups not configured
- Challenge favorite icons not displaying
- 'Signup school not listed' icon not displaying
- Badge display on the dashboard to wrap properly

### Removed

- Old prior-version FontAwesome files

## [4.1.0] - 2018-11-30

### Added

- Logging prior to spin-up of Web server (Fix #241)
- Logging when site managers authenticate and when test emails are sent
- Ability for Serilog to output to a SQL Server table
- Application setting to support SQL Server 2008
- Database context pooling
- Request trace identifier to logging
- Reverse proxy address configuration option
- Robots noindex and nofollow to Mission Control pages (to prevent search engine crawling)

### Changed

- Upgrade from ASP.NET Core v1.1 to v2.1
- Upgrade Font Awesome from v4.7.0 to v5.5.0

### Fixed

- Docker builds with Travis CI
- Allow maximum allowable activity to be configured without requiring code changes

### Removed

- Hack to get around Antiforgery issue (Fix #357)
- Structure for having a "no avatars" release build

## [4.0.0] - 2018-10-09

### Added

- Mission Control location list shows a count of related events/community experiences (#322)
- Add Google Analytics Event when participant joins (#324)
- Survey linking for all participants and first time participants
- Participant Count and Minutes by Program Report
- First time users to Activity by Program Report
- Community experience attendance report
- Email address collection after program has ended
- Templates in the shared folder for front page and dashboard views
- First time participant count on vendor code report
- Achiever count to the participant count and minutes by program report

### Changed

- Mission Control events navbar icon is now a drop-down (#322)
- Fix automated builds and pushing to Docker Hub
- Developer ability to add minute cap to reporting
- Whether or not to collect email addresses prior to registration is now a site setting

### Fixed

- Set page title to "community experiences" when selected on the events page
- Registrations and achievers by school report name (#320)
- Issue showing list of badges for the Badge Top Scores Report
- Public event locations list sorts alphabetically (#321)
- Default start and end date on reports which require date selection (#317)
- Button on reports with date selection for the entire program (#317)
- Button on reports with date selection for the last week (#317)
- Report criteria to show on browser report view and exports (#318)
- Mail threading showing html as text
- Disable avatar save button while saving to avoid accidental double-clicks
- Issue with parsing some Excel spreadsheets in the vendor code status update
- Reporting Excel sheet formatting error
- Issue displaying vendor code notes on household pages

## [4.0.0-beta2] - 2018-05-18

### Added

- Group Vendor Code Report: group members and their assigned vendor codes
- Admins with Edit Participant permission can upgrade any households to groups
- Point translation management in Mission Control
- SQL Server create database script in db/ folder
- Documentation in docs/ folder to update the manual for version 4
- Metadata description and structured data for Events
- Ability to restrict triggers from activating until a specified date
- Role management
- Authorization code management
- Participant/role assignment
- Automatically assign any new permissions to the System Administrator role
- User-friendly page to display page not found (404) errors
- Ability to send test emails from Mission Control
- Token {Link} in vendor code mails
- Vendor code donations report
- Default culture so as to not show 24-hour time in event details

### Changed

- Updated package dependencies to latest compatible versions
- Improve descriptions on Site Management pages
- Updated front-end packages to latest compatible versions
- Display event title as hyperlink rather than URL
- Automatically select school district during registration if there's only one
- Improve avatar share pages

### Fixed

- Broken household URLs in Mission Control (leading spaces)
- Promoting member to household lead wasn't transferring the group
- Ability to assign groups to household members (not just the lead)
- Dashboard error caused by daily literacy tips
- Disable add default avatars button on click
- Misspelled app setting (GraApplicationDiscriminator)
- Handle admin users self-deleting properly (log out, redirect to front of site)
- Issue with double initial page loads causing duplicate database insertions (#283)
- Editing a trigger that doesn't exist now shows an error rather than logging an exception
- Don't need manage vendor code permission to list vendor code types (for reporting)
- User admin status is updated as their roles are updated
- Bug preventing deletion of challenge groups

### Removed

- Comments from appsettings.json, see the
  [manual](http://manual.greatreadingadventure.com/en/latest/technical/appsettings/) for more
  information
- Activity earned from at a glance report

## [4.0.0-beta1] - 2018-04-13

### Added

- Everything! First release of 4.0.

[4.6.0]: https://github.com/mcld/greatreadingadventure/tree/v4.6.0
[4.5.0]: https://github.com/mcld/greatreadingadventure/tree/v4.5.0
[4.4.2]: https://github.com/mcld/greatreadingadventure/tree/v4.4.2
[4.4.1]: https://github.com/mcld/greatreadingadventure/tree/v4.4.1
[4.4.0]: https://github.com/mcld/greatreadingadventure/tree/v4.4.0
[4.3.17]: https://github.com/mcld/greatreadingadventure/tree/v4.3.17
[4.2.1]: https://github.com/mcld/greatreadingadventure/tree/v4.2.1
[4.2.0]: https://github.com/mcld/greatreadingadventure/tree/v4.2.0
[4.1.1]: https://github.com/mcld/greatreadingadventure/tree/v4.1.1
[4.1.0]: https://github.com/mcld/greatreadingadventure/tree/v4.1.0
[4.0.0]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0
[4.0.0-beta2]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0-beta2
[4.0.0-beta1]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0-beta1
