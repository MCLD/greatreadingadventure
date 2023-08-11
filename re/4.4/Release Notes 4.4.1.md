## Release Notes: 4.4.1

### Introduction

The Great Reading Adventure is a robust, open source software designed to manage library reading programs. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) for an overview of its functionality and capabilities.

### Release

These release notes accompany **[:books: The Great Reading Adventure version 4.4.1](https://github.com/MCLD/greatreadingadventure/releases/download/v4.4.1/GreatReadingAdventure-4.4.1.zip)** which can be downloaded from GitHub or run in Docker from the [Docker Hub](https://hub.docker.com/r/mcld/gra) or [GitHub Packages](https://github.com/MCLD/greatreadingadventure/pkgs/container/gra).

### Upgrading

If you wish to upgrade from a previous release of version 4 see the [upgrading section](http://manual.greatreadingadventure.com/en/v4.4.1/installation/upgrading/) of the manual. There is no upgrade path from versions 2 or 3 to this release due to significant architectural changes.

### Documentation

For information on what is required to run the Great Reading Adventure, please refer to the [online manual](http://manual.greatreadingadventure.com/en/v4.4.1/). The software requires a [hosting environment including Microsoft .NET 7.0 and Microsoft SQL Server](http://manual.greatreadingadventure.com/en/v4.4.1/installation/system-requirements/). If you don&rsquo;t have such an environment, there are plenty of Web hosting services you can use.

### Changes in this release

Here are some notable changes:

- Configurable side-wide point goal displayed on dashboard to participants
- Better supports for Google Analytics 4 and events
- Decouple vendor codes from participants; allow a participant to be reassigned a code if needed
- Handle participant reassignment if their branch is deleted
- Packing slip improvements in display and ability to print hold slips
- View user change history from Mission Control
- Prize view for participants
- Allow assocation of a certificate (PDF file) to a trigger
- Mission Control news posts can now be updated and jumped back to the top of the display queue
- Allow vendor code list upload to invalidate and assign new codes to customers
- Return proper HTTP codes on page errors

For more detailed information about changes in this release please review the [change log](https://github.com/MCLD/greatreadingadventure/blob/v4.4.1/CHANGELOG.md).

### Known issues

For up-to-date information about known issues please refer to [the discussions](https://github.com/MCLD/greatreadingadventure/discussions) and the GitHub [issue list](https://github.com/MCLD/greatreadingadventure/issues).
