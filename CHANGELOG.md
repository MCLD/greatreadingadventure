# Change log
All notable changes to this project will be documented in this file.

## [Unreleased]
### Added
- Link from badges to search events which can earn them
- Add QueryString capabilities to events index
  - `/Events/?Branch=<branch name>` shows events for that branch
  - `/Events/?System=<system name>` shows events for that library system
  - `/Events?Search=<search string>` shows events with the provided search string
  - `/Challenges?Search=<search string>` shows challenges with the provided search string
- #52 Add field to enter book details for book review. The field's visibilty is controlled by a program flag.
- Patron registration process now informs users about goal range.

### Changed
- #151 Reading goal now only accounts for points earned through reading instead of all point methods

### Fixed
- #153 Control room avatar editor now syas "Avatar name", instead of "Award NAme"

### Changed
- Award Triggers no longer require a point value to be entered
- Reorder and colorize buttons on profile and family list to make them more logical
- Show family point values on family list
- Show event structured data in the configured time zone rather than UTC

### Fixed
- Took steps to minimize ViewState size on patron pages
- Patrons can no longer register early by guessing the registration page url (#149)
- Removed 50 character length requirement on Challenge names

### Removed
- Remove sharing buttons from hidden badges (they can't be shared anyway!)

## [3.1.0] - 2016-04-27
### Added
- Feature for optionally requiring users to enter book details
- #54 Integrate badge tool
- #56 Ability to bulk-add codes with crosswalks
- #57 Ability to hide badges and events from public galleries
- #60 CR drawings: selected winner text is static, should be a link to the user's profile
- #64 Allow patrons to specify their own goal
- #69 Add structured data for public events display
- #76 Control room should have an ability to send a test email
- #80 Add at-a-glance report
- #88 Add ability to add existing accounts as family members
- #94 Customizable avatars
- #97 Add event import and export from/to Excel
- #99 Add ability to use a direct link to provide a patron with a secret code
- #109 Allow a program default for the "set your own goal" functionality
- #118 Allow personal goal to be configured as "monthly", "weekly" or "daily" goal
- Social media sharing - easily share avatars, badges, challenges, and events on Facebook or Twitter

### Changed
- #22 Badge details should show if the current patron has been awarded the badge and when
- #61 Improve Events and their management
- #65 "Badge Awards" is confusing, rename to "Award Triggers"
- #55 Control room needs search and filter abilities
- #78 Control room reporting enhancements
- #102 Add Goal information to description of how badges are earned.
- #106 Challenges publicly viewable; immutable once achieved
- #128 Add search to patron challenge view
- #129 Move Choose-Your-Own images to bottom underneath text.
- #132 Award Triggers should allow "x of y" badges to trigger, not just all the checked badges

### Fixed
- #24 Events with no end date do not display and codes for them do not function
- #58 CR users without notification permissions shouldn't be able to ever send notifications
- #59 CR events - secret code redundancy checker does a postback and clears fields
- #72 Updating the school crosswalk causes an error
- #86 Caching for showing site areas doesn't work across tenants
- #87 Badges which are not square cause the gallery rows to not line up
- #100 Start date/log start date are not being followed properly
- #104 Fix issues with literacy pretest
- #110 Once a prize is marked redeemed it cannot be marked as unredeemed
- #112 Error when trying to edit Tenant admin user.
- #113 Parental consent text displays HTML tags.
- #115 Fields must be set as "Allow Edit" in Registration Settings for CR admins to edit them
- #121 Using My Account -> Add Family Member -> Register a new family member fails
- #127 Ensure patron family member list is alphabetical by first name
- #130 Editing a user in the CR Patron section resets their password

## [3.0.3] - 2016-02-18
### Fixed
- Fix #83 Patron sub-account list causes error, patron details -> patron tab causes error
- Fix #82 HTML entered in Organization configuration isn't rendered on the selection screen
- Add missing stored procedure app_Notifications_GetAllToOrFromPatron

### Security
- Improve patron password recovery logging

## [3.0.2] - 2016-02-10
### Fixed
- Fix #72 - Updating the school crosswalk causes an error
- Fix #73 - Selecting "Save" multiple times in School/District Setup causes duplicate crosswalk records

## [3.0.1] - 2016-02-01
- Improve and simplify UX for patrons using the GRA (#11 among other issues)
  - Incorporate Bootstrap 3.3.5, jQuery 2.2.0
  - Substantial and significant redo of all user-facing pages and controls (including Adventures)
  - Remove program selection from front page of site
  - Remove unknown Facebook integration bits
  - *Mini-games* are now *Adventures*
  - *Book lists* are now *Challenges*
  - *Notifications* are now *Mail*
  - Badge details are now shown in a uniform pop-up window
  - Pop-ups for events and challenges with liberal use of Ajax
  - Show and hide navigation based on configuration (e.g. if there are no badges configured, don't show navigation to the badge gallery).
  - Add print buttons
  - Badges and events are shown publicly to everyone now
  - Individual badge and event public pages to improve search engine visibility
  - Modify Challenges (formerly book lists) to not require book information
- Sane defaults for Activity Point Conversions (#13).
- All passwords are now hashed (#7).
- Session timeouts no longer default users to the master tenant (#8).
- Administrative logins now do not experience "session hopping" (#9).
- Control Room logins are now fully separated from user logins (#10).
- Fix issues program selection based on ages (#14, #16).
- Resolve issue with adding a tenant if email fails (#15).
- Redone initial configuration process.
- Ability to select a default install with one program or four programs.
- Added capability for express setup with ILS integration (proofed out for [Polaris](https://iii.com/products/polaris) but adaptable to other ILS software).
- Add printable "Book List" showing every title that a user has read (if they choose to enter titles and authors)
- Revised art replacing existing art of unknown origin and copyright status

## [3.0.0] - 2016-02-01 [YANKED]

## [2.2.1] - 2015-09-01
- This is a full release of the software not requiring any upgrades to get to the latest version
- If there are no tenants (only the master tenant) then patrons are taken directly to the master tenant's reading program eliminating an empty tenant selection drop-down.
- Removal of unused files throughout the project
- Dependencies updated to the latest versions
- Add the ability to work against LocalDB (if it's installed) for testing or development
- Add logging capabilities

## [2.2.0] - 2015-06-05

## [2.1.0] - 2015-06-05
