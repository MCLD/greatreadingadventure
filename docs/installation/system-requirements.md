# System requirements

The GRA requires the following services to run. These services may all be hosted on the same machine
or may be hosted on separate machines for performance reasons.

## Web server
[Microsoft Internet Information Services (IIS)](https://www.iis.net/) version 7.0
  or later.
  * Windows Server 2008 shipped with IIS 7 making it the minimum requirement for operating system.
  * The [.NET Framework](https://msdn.microsoft.com/en-us/vstudio/aa496123.aspx) version 4.0 or later must be installed with IIS configured to run ASP.NET pages.
  * Ideally, the GRA runs in its own Application Pool configured in the **Integrated managed pipeline** mode.
  * File management (i.e. uploading images, configuration changes, etc.) occur by allowing the account running the App Pool to have write access to the Web server files on disk. In many configurations, that means that the `IIS_IUSRS` group should be granted write access to the file system.

## Database server
[Microsoft SQL Server](http://www.microsoft.com/en-us/server-cloud/products/sql-server/) 2008 or later.
  * The GRA uses its own database with its own database users so it can easily and safely coexist with other SQL Server databases.
  * SQL Server must allow connections via [SQL Server authentication mode](https://msdn.microsoft.com/en-us/library/ms144284.aspx).
  * The GRA requires an empty database to be created manually or [with a script](https://raw.githubusercontent.com/MCLD/greatreadingadventure/master/CreateDatabase.sql).
  * The GRA also requires two users for initial setup:
    * A database user with owner access (role `db_owner`) for creating the initial database structures and inserting the initial data. This user can be removed once the application is installed.
    * A database user with read/write access (roles `db_datareader` and `db_datawriter`) to the database for running the GRA software. This user will also need to be able to execute stored procedures, the setup process should grant them that privilege.

## Mail server
The ability to send Internet email, such as a service which accepts email via SMTP.
  * The GRA sends mail in certain instances (such as helping users recover their lost passwords) and requires the ability to connect to an SMTP server.
  * The SMTP server built-in to IIS can be used, any other on-site mail server can be used (Microsoft Exchange Server, Postfix, etc.), or an external mail service can be used.