# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## Unreleased
### Added
- Enhance date selection on reports

### Fixed
- Fix page title to "community experiences" when selected on the events page

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

[4.0.0-beta2]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0-beta2
[4.0.0-beta1]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0-beta1
