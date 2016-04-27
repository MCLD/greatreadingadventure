## Release Notes: 3.0.1

### Introduction

The Great Reading Adventure is a robust, open source software designed to manage library reading programs. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) for an overview of its functionality and capabilities.

### Release

These release notes accompany **[:books: The Great Reading Adventure version 3.0.1](https://github.com/MCLD/greatreadingadventure/releases/download/v3.0.1/GreatReadingAdventure-3.0.1.zip)** which can be downloaded from GitHub!

This is a **full release** of the full software package. We currently **do not** have an upgrade path to go from prior versions to this version due to architectural changes. If you have a critical need for the ability to upgrade, please [post in the Help category on the forum](http://forum.greatreadingadventure.com/c/help) and we will try to work something out.

### Documentation

For information on what is required to run the Great Reading Adventure, please refer to the [online manual](http://manual.greatreadingadventure.com/).

### Changes in this release

- Improve and simplify UX for patrons using the GRA (#11 among other issues)
  - Incorporate Bootstrap 3.3.5 and jQuery 2.2.0
  - Substantial and significant redo of all user-facing pages and controls
  - Remove program selection from front page of site
  - Remove unknown Facebook integration bits
  - *Mini-games* are now *Adventures*
  - *Book lists* are now *Challenges*
  - *Notifications* are now *Mail*
  - More obvious unread mail indicator badge
  - Badge details are now shown in a uniform pop-up window site-wide
  - Alert system for showing notifications in a uniform way
  - Pop-ups for events and challenges with liberal use of Ajax
  - Show and hide navigation based on configuration (e.g. if there are no badges configured, don't show navigation to the badge gallery).
  - Add relevant print buttons
  - Badges and events are shown publicly to everyone
  - Individual badge and event public pages to improve search engine visibility
  - Modify Challenges to not require book information
- Sane defaults for Activity Point Conversions (#13).
 All passwords are now hashed (#7).
- Session timeouts no longer default users to the master tenant (#8).
- Administrative logins now do not experience "session hopping" (#9).
- Control Room logins are now fully separated from user logins (#10).
- Fix issues program selection based on ages (#14, #16).
- Resolve issue with adding a tenant if email fails (#15).
- Improved initial configuration process.
- Ability to select a default install with one reading program or four reading programs.
- Added capability for express setup with ILS integration (proofed out for [Polaris](https://iii.com/products/polaris) but adaptable to other ILS software through an abstract proxy mechanism).
- Add printable "Book List" showing every title that a patron has read (if they entered titles and authors).
- Replace "leaderboard" with "feed" showing recent point-earning accomplishments by other patrons and "status" to show aggregate numbers
- Remove some Adventures (Hidden Picture, Matching Game, Word Match) and enhance the remaining ones (Choose Your Adventure, Code Breaker, Mix-And-Match, Online Book).
- Plenty more, [this pull request](https://github.com/MCLD/greatreadingadventure/pull/27) covers a lot of it.

### Known issues

These are some of the more significant issues which have already been reported. Feel free to post to [the forum](http://forum.greatreadingadventure.com/) with more information or new issues. GitHub always contains the up-to-date [technical developer view of issues and bug reports](https://github.com/MCLD/greatreadingadventure/issues).

- #23 With JavaScript off, enter on the login page should submit the login form
- #24 Events with no end date do not display and codes for them do not function
- #28 Libraries/Schools don't function unless they are a part of a district
- #29 Prompt user for sysadmin password during configure
- #38 Enter book details panel does not respond to "Enter"
- #72 Updating the school crosswalk causes an error
- #73 Selecting "Save" multiple times in School/District Setup causes duplicate crosswalk record
