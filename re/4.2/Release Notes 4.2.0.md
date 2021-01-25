## Release Notes: 4.2.0

### Introduction

The Great Reading Adventure is a robust, open source software designed to manage library reading programs. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) for an overview of its functionality and capabilities.

### Release

These release notes accompany **[:books: The Great Reading Adventure version 4.2.0](https://github.com/MCLD/greatreadingadventure/releases/download/v4.2.0/GreatReadingAdventure-4.2.0.zip)** which can be downloaded from GitHub or run in Docker from the [Docker Hub](https://hub.docker.com/r/mcld/gra) or the [GitHub Container Registry](https://ghcr.io/mcld/gra).

### Upgrading

If you wish to upgrade from a previous release of version 4 see the [upgrading section](http://manual.greatreadingadventure.com/en/v4.2/installation/upgrading/) of the manual. There is no upgrade path from versions 2 or 3 to this release due to significant architectural changes.

### Documentation

For information on what is required to run the Great Reading Adventure, please refer to the [online manual](http://manual.greatreadingadventure.com/en/v4.2/). The software requires a [hosting environment including the Microsoft .NET 5.0 framework and Microsoft SQL Server](http://manual.greatreadingadventure.com/en/v4.2/installation/system-requirements/). If you don&rsquo;t have such an environment, there are plenty of Web hosting services you can use.

### Changes in this release

There are a lot of changes in this release, here are some of the more notable ones:

- Update to the Microsoft .NET 5.0 framework
- Internationalization of the customer-facing site including Spanish language translations
- Add method for staff to activate unfired triggers via Mission Control
- Schedulable carousel of materials or links can now be placed on the dashboard
- Functionality to allow performers to self-register and staff to select performances for their locations
- News area (with categories) of Mission Control for providing communications to staff with email subscriptions
- Proximity search for events (i.e. 'find events near me')
- Add exit page to provide call-to-action after logout
- Built-in job system using WebSockets so long-running processes don't time out (e.g. avatar import, reports, sending emails, etc.)
- Ability to send emails from in the software to people who have signed up for pre- or post-program lists and participants
- Allow participants to add books to their book list independent of logging activity
- Add maximum file size, dimensional size, and square dimensional requirement for badge images
- New streaming event category with ability to allow scheduled viewing of streams
- Lots of bug fixes and performance improvements

For more detailed information about changes in this release please review the [change log](https://github.com/MCLD/greatreadingadventure/blob/v4.2.0/CHANGELOG.md) and [related issues on GitHub](https://github.com/MCLD/greatreadingadventure/milestone/12?closed=1).

### Known issues

For up-to-date information about known issues please refer to [the discussions](https://github.com/MCLD/greatreadingadventure/discussions) and the GitHub [issue list](https://github.com/MCLD/greatreadingadventure/issues).
