## Release Notes: 4.5.0

### Introduction

The Great Reading Adventure is a robust, open source software designed to manage library reading programs. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) for an overview of its functionality and capabilities.

### Release

These release notes accompany **[:books: The Great Reading Adventure version 4.5.0](https://github.com/MCLD/greatreadingadventure/releases/download/v4.5.0/GreatReadingAdventure-4.5.0.zip)** which can be downloaded from GitHub or run in Docker from the [Docker Hub](https://hub.docker.com/r/mcld/gra) or [GitHub Packages](https://github.com/MCLD/greatreadingadventure/pkgs/container/gra).

### Upgrading

If you wish to upgrade from a previous release of version 4 see the [upgrading section](http://manual.greatreadingadventure.com/en/v4.5.0/installation/upgrading/) of the manual. There is no upgrade path from versions 2 or 3 to this release due to significant architectural changes.

### Documentation

For information on what is required to run the Great Reading Adventure, please refer to the [online manual](http://manual.greatreadingadventure.com/en/v4.5.0/). The software requires a [hosting environment including Microsoft .NET 8.0 and Microsoft SQL Server](http://manual.greatreadingadventure.com/en/v4.5.0/installation/system-requirements/). If you don&rsquo;t have such an environment, there are plenty of Web hosting services you can use.

### Changes in this release

Notable changes:

- Upgrade to ASP.NET Core version 8.0

Notable changes from the prior release:

- Retain participant challenge filter selections in the session
- Change display of "Program" on the participant side to "Age Group"
- Welcome email capability with sending delay after program signup, maximum number of emails limit
- Fix avatar scrolling bug at mobile responsive level
- Streamlined account creation abilities via authorization codes
- Performer scheduling enhancements including coversheet generation, streaming improvements
- Performer spreadsheet export
- Job system with scheduled tasks for sending emails
- Improve hold slips to include age group and group name, allow generation without marking received
- Fix issues resulting in participants having multiple assigned vendor codes

For more detailed information about changes in this release please review the [change log](https://github.com/MCLD/greatreadingadventure/blob/v4.5.0/CHANGELOG.md).

### Known issues

For up-to-date information about known issues please refer to [the discussions](https://github.com/MCLD/greatreadingadventure/discussions) and the GitHub [issue list](https://github.com/MCLD/greatreadingadventure/issues).
