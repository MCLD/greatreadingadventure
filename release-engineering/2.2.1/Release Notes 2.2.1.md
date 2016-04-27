## Release Notes: 2.2.1

### Introduction

The Great Reading Adventure is a robust, open source software designed to manage library reading programs. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) for an overview of its functionality and capabilities.

### Release

These release notes accompany The Great Reading Adventure version 2.2.1. A [pre-built package](https://github.com/MCLD/greatreadingadventure/releases/download/v2.2.1/GreatReadingAdventure-2.2.1.zip) is available from GitHub.

### Highlights of this release

* This is a full release of the software not requiring any upgrades to get to the latest version
* If there are no tenants (only the master tenant) then patrons are taken directly to the master tenant's reading program eliminating an empty tenant selection drop-down.
* Removal of unused files throughout the project
* Dependencies updated to the latest versions
* Add the ability to work against LocalDB (if it's installed) for testing or development
* Add logging capabilities

### Known issues

These are some of the more important issues which have already been reported. Please add comments to the existing GitHub issues if you have more details to provide.

* After installing, each program must have the top section of the *Activity Point Conversions & Literacy Testing* populated (i.e. "x minutes EQUALS y POINTS") or patrons will not be able to log minutes (GitHub issue #13)
* Customer passwords are stored in cleartext (GitHub issue #7)
* If a user experiences a session timeout, further browser action defaults them to the master tenant (GitHub issue #8)
* Administrative logins can experience "session hopping" (GitHub issue #9)
* ControlRoom tenant selection can be affected by public program selection (GitHub issue #10)
