# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## Unreleased
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
- Admins with Edit Participant permission can upgrade households to groups regardless of household size
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
- Manage vendor code permission no longer required to get a list of vendor code types (for reporting)
- User admin status is updated as their roles are updated
- Bug preventing deletion of challenge groups

### Removed
- Comments from appsettings.json, see the [manual](http://manual.greatreadingadventure.com/en/latest/technical/appsettings/) for more information
- Activity earned from at a glance report

## [4.0.0-beta1] - 2018-04-13
### Added
- Everything! First release of 4.0.

[4.0.0]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0
[4.0.0-beta2]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0-beta2
[4.0.0-beta1]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0-beta1
