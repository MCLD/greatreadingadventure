## Release Notes: 4.0.0-beta2

### Introduction

The Great Reading Adventure is a robust, open source software designed to manage library reading programs. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) for an overview of its functionality and capabilities.

### Release

These release notes accompany **[:books: The Great Reading Adventure version 4.0.0-beta2](https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0-beta2/GreatReadingAdventure-4.0.0-beta2.zip)** which can be downloaded from GitHub!

Avatar assets extracted from [Glitch the Game](https://www.glitchthegame.com/) are packaged in [defaultavatars.zip](https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0-beta1/defaultavatars.zip). Instructions for importing these avatars can be found in the [manual](http://manual.greatreadingadventure.com/).

### Upgrading

**There is no upgrade path from versions 2 or 3 to this release** due to significant architectural changes.

Upgrading is possible from deployed instances of version 4. Note that during the upgrade there will be an interruption in service so it may be ideal to schedule this upgrade in off hours. To upgrade from prior versions (i.e. 4.0.0-beta1) follow these steps:

1. Back up the database and the files that comprise the Web site.
2. If you have modified any files in the application (such as `.cshtml` Razor template files), please make a note of which changes you've made, these changes will have to be performed again. Changes to the `shared` directory (such as to `style.css` or uploaded files) will be maintained.
3. Replace the application files with the files from this release. **Ensure that your `shared` directory is not overwritten when replacing application files**.
4. Load the site. It will take longer than normal as the database is upgraded (you can see evidence of the database upgrade in the log file in `shared/logs`).

### Documentation

For information on what is required to run the Great Reading Adventure, please refer to the [online manual](http://manual.greatreadingadventure.com/). The software requires a [hosting environment including the Microsoft .NET Core 1.1 framework and Microsoft SQL Server](http://manual.greatreadingadventure.com/en/latest/installation/system-requirements/). If you don't have such an environment, there are plenty of Web hosting services you can use.

### Changes in this release

Notable changes in this release:

- Households can now be upgraded to groups by administrators regardless of household size
- Structured data for events
- Friendly 404 page
- Added the ability to set a culture for displaying dates and times (defaulting to 'en-US' with 12-hour times)
- Automatically select School District during registration if there's only one
- Mission Control management of point translations, authorization codes, roles, participant role assignments
- Ability to send test emails from Mission Control
- Vendor code improvements (reports, additional email token)
- Fixed issues with household leads and groups in Mission Control

For more detailed information about changes in this release please review the [change log](https://github.com/MCLD/greatreadingadventure/blob/v4.0.0-beta2/CHANGELOG.md).

### Known issues

Here's a selection of some notable currently-known issues:

- Once a broadcast is created it cannot be deleted, only superseded with subsequent broadcasts (issues [#308](https://github.com/MCLD/greatreadingadventure/issues/308) and [#309](https://github.com/MCLD/greatreadingadventure/issues/309)).
- Avatar management is designed to use our Glitch avatar package, full avatar management isn't functional yet (issue [#261](https://github.com/MCLD/greatreadingadventure/issues/261)).


For up-to-date information about known issues please refer to [the forum](http://forum.greatreadingadventure.com/) and the GitHub [issue list](https://github.com/MCLD/greatreadingadventure/issues).
