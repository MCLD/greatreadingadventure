## Release Notes: 4.0.0

### Introduction

The Great Reading Adventure is a robust, open source software designed to manage library reading programs. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) for an overview of its functionality and capabilities.

### Release

These release notes accompany **[:books: The Great Reading Adventure version 4.0.0](https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0/GreatReadingAdventure-4.0.0.zip)** which can be downloaded from GitHub!

Avatar assets extracted from [Glitch the Game](https://www.glitchthegame.com/) are packaged in [defaultavatars.zip](https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0/defaultavatars.zip). Instructions for importing these avatars can be found in the [&ldquo;Adding Avatars&rdquo; section of the manual](http://manual.greatreadingadventure.com/en/latest/setup/adding-avatars/).

### Upgrading

**There is no upgrade path from versions 2 or 3 to this release** due to significant architectural changes.

Upgrading is possible from deployed instances of version 4 (i.e. 4.0.0-beta1 and 4.0.0-beta2). Note that during the upgrade there will be an interruption in service so it may be ideal to schedule this upgrade in off hours. To upgrade from prior versions follow these steps:

1. Back up the database and the files that comprise the Web site.
2. If you have modified any files in the application (such as `.cshtml` Razor template files), please make a note of which changes you&rsquo;ve made, these changes will have to be performed again. Changes to the `shared` directory (such as to `style.css` or uploaded files) will be maintained.
3. Replace the application files with the files from this release. **Ensure that your `shared` directory is not overwritten when replacing application files**.
4. The `appsettings.json` file in the application folder will be replaced when you copy in files from the release. If you modified this file please compare to the file you backed up in the first step above to ensure any configuration changes you made initially are set in the new file. If you added an `appsettings.json` file in the `shared` directory with your settings it will **not be overwritten** and you shouldn&rsquo;t need to change any settings.
5. Load the site. It will take longer than normal as the database is upgraded (you can see evidence of the database upgrade in the log file in `shared/logs`).

### Documentation

For information on what is required to run the Great Reading Adventure, please refer to the [online manual](http://manual.greatreadingadventure.com/). The software requires a [hosting environment including the Microsoft .NET Core 1.1 framework and Microsoft SQL Server](http://manual.greatreadingadventure.com/en/latest/installation/system-requirements/). If you don&rsquo;t have such an environment, there are plenty of Web hosting services you can use.

### Changes in this release

Notable changes in this release:

- If Google Analytics is configured, an event triggers when a participant joins (track the source of sign-ups)
- Ability to add links to external survey (can be a different link for first time participants)
- Added new and enhanced existing reports, improved reporting date selection, showing report criteria on report and in exports
- Added templates for front page and dashboard views to the shared folder to make customization easier

For more detailed information about changes in this release please review the [change log](https://github.com/MCLD/greatreadingadventure/blob/v4.0.0/CHANGELOG.md).

### Known issues

Here&rsquo;s a selection of some notable currently-known issues:

- Once a broadcast is created it cannot be deleted, only superseded with subsequent broadcasts (issues [#308](https://github.com/MCLD/greatreadingadventure/issues/308) and [#309](https://github.com/MCLD/greatreadingadventure/issues/309)).
- Avatar management is designed to use our Glitch avatar package, full avatar management isn&rsquo;t functional yet (issue [#261](https://github.com/MCLD/greatreadingadventure/issues/261)).

For up-to-date information about known issues please refer to [the forum](http://forum.greatreadingadventure.com/) and the GitHub [issue list](https://github.com/MCLD/greatreadingadventure/issues).
