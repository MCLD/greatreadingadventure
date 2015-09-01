# The Great Reading Adventure
The Great Reading Adventure is a robust, open source software designed to manage library reading programs. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) for an overview of its functionality and capabilities.

You can view the latest [release notes](https://github.com/MCLD/greatreadingadventure/releases/latest) or [download the latest version (2.2.1)](https://github.com/MCLD/greatreadingadventure/releases/download/v2.2.1/GreatReadingAdventure-2.2.1.zip).

See the [SourceForge](http://sourceforge.net/projects/thegreatreadingadventure/) site for [some documentation](http://sourceforge.net/p/thegreatreadingadventure/wiki/Home/).

You can visit the [Great Reading Adventure Forum](http://forum.greatreadingadventure.com/) for the following:

* [Help with installation](http://forum.greatreadingadventure.com/c/install-issues)
* [Assistance setting up and using the GRA](http://forum.greatreadingadventure.com/c/help)
* [Reporting errors](http://forum.greatreadingadventure.com/c/errors)
* [Suggesting new features](http://forum.greatreadingadventure.com/c/feature-requests)

There are also some additional [requested features in this document](https://docs.google.com/spreadsheets/d/1n4vCkW0WWNyRh3dvPy5eBwGedzXQ4RnjuZQTuPKJ-Bg/edit?usp=sharing).

## Development
GRA code should successfully compile using the free [Visual Studio Community](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) edition. External dependencies are managed with NuGet and should be automatically downloaded upon the first build.

By default, GRA is configured to use LocalDB as a database for development and testing. While not packaged with Visual Studio Community, it can easily be added by selecting the notification flag in the title bar of Visual Studio and choosing to install the "MS SQL Server Update for database tooling". Alternately, the [SQL Server Data Tools (SSDT)](https://msdn.microsoft.com/en-us/library/mt204009.aspx) package can be installed manually by downloading it from Microsoft.

If you have any trouble getting the package to build, please submit an [issue](https://github.com/MCLD/greatreadingadventure/issues/new) with details.

## License

The Great Reading Adventure source code is distributed under [The MIT License](http://opensource.org/licenses/MIT). For other included packages and those fetched by NuGet, please see the LICENSE file in the project.

The Great Reading Adventure was initially developed by the [Maricopa County Library District](http://www.mcldaz.org/) with support by the [Arizona State Library, Archives and Public Records](http://www.azlibrary.gov/), a division of the Secretary of State, with federal funds from the [Institute of Museum and Library Services](http://www.imls.gov/).
