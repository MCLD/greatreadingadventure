## Release Notes: 4.0.0

### Introduction

The Great Reading Adventure is a robust, open source software designed to manage library reading programs. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) for an overview of its functionality and capabilities.

### Release

These release notes accompany **[:books: The Great Reading Adventure version 4.0.0](https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0/GreatReadingAdventure-4.0.0.zip)** which can be downloaded from GitHub!

Two release packages are available:

- The [v4.0.0 release package](https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0/GreatReadingAdventure-4.0.0.zip) includes over 4,000 avatar assets extracted from [Glitch the Game](https://www.glitchthegame.com/).
- The [v4.0.0-noavatars release package](https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0/GreatReadingAdventure-4.0.0-noavatars.zip) excludes avatar assets (they can be downloaded and added later from the [defaultavatars-4.0.0](https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0/defaultavatars-4.0.0.zip) file).

For more information on installing and configuring avatars, see the [avatar section](http://manual.greatreadingadventure.com/en/v4.0.0/setup/adding-avatars/) of the manual.

### Upgrading

**There is no upgrade path from versions 2 or 3 to this release** due to significant architectural changes. If you wish to upgrade from a previous release of version 4 (i.e. 4.0.0-beta1 and 4.0.0-beta2) see the [upgrading section](http://manual.greatreadingadventure.com/en/v4.0.0/installation/upgrading/) of the manual.

### Documentation

For information on what is required to run the Great Reading Adventure, please refer to the [online manual](http://manual.greatreadingadventure.com/). The software requires a [hosting environment including the Microsoft .NET Core 1.1 framework and Microsoft SQL Server](http://manual.greatreadingadventure.com/en/v4.0.0/installation/system-requirements/). If you don&rsquo;t have such an environment, there are plenty of Web hosting services you can use.

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
