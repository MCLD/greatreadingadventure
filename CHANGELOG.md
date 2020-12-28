# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## Unreleased

## [4.2.0] - 2020-12-31

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

[4.2.0]: https://github.com/mcld/greatreadingadventure/tree/v4.2.0
[4.1.1]: https://github.com/mcld/greatreadingadventure/tree/v4.1.1
[4.1.0]: https://github.com/mcld/greatreadingadventure/tree/v4.1.0
[4.0.0]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0
[4.0.0-beta2]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0-beta2
[4.0.0-beta1]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0-beta1
