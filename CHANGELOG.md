# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## [Unreleased]
### Added
- Group Vendor Code Report: group members and their assigned vendor codes
- Admins with Edit Participant permission can upgrade households to groups regardless of household size
- Point translation management in Mission Control
- SQL Server create database script in db/ folder
- Documentation in docs/ folder to update the manual for version 4
- Metadata description and structured data for Events

### Fixed
- Broken household URLs in Mission Control (leading spaces)
- Promoting member to household lead wasn't transferring the group
- Ability to assign groups to household members (not just the lead)
- Dashboard error caused by daily literacy tips
- Disable add default avatars button on click
- Misspelled app setting (GraApplicationDiscriminator)
- Logout admin users on self deletion

### Removed
- Comments from appsettings.json, see the [manual](http://manual.greatreadingadventure.com/en/latest/technical/appsettings/) for more information

## [4.0.0-beta1] - 2018-04-13
### Added
- Everything! First release of 4.0.

[4.0.0-beta1]: https://github.com/mcld/greatreadingadventure/tree/v4.0.0-beta1

