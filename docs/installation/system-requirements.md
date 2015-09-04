# System Requirements

Microsoft Windows Server 2008 R2 or later

IIS Web Server 7 or later

* Running on an application pool configured in Classic managed pipeline mode, with .NET Framework
v4.0.x.
* The root folder physical path, configured with Modify permissions for the IIS_IUSRS user. (Web
sites can be configured many different ways, including customizing the user id of the web process.
Whatever that user is, id different than IIS_IUSRS will need Modify permission to the root
folder physical path).

SQL Server 2008 or later

* The database can run on the same or a different physical or virtual machine as long as network
connectivity can be established between the two machines.
* The SQL Server is configured with SQL Server authentication mode.
* An empty database has been created for the application database (i.e. SRP_Web).
* One SQL Server authentication login, with dbo security privileges to the application database.
This user will be used to create the database objects and populate the initial data.One SQL
Server authentication login, with read/write security privileges to the application database. This
user will be used to access the database at runtime.
* NOTE: At this point, SQL Server Express, SQLite, and MySQL will not work (but it would be great if you could modify the install scripts to make them work).  

Mail server

* Configured to send out email on behalf of the web application. This can be the local SMTP service
on the webserver, or a remote relay server. Regardless, make note of the server name/
address.