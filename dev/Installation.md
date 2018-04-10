### Initial Set Up and Planning

**Note: more detailed [installation instructions](http://manual.greatreadingadventure.com/en/latest/installation/getting-started.html) are provided in the [Great Reading Adventure manual](http://manual.greatreadingadventure.com/), however they are lacking screenshots.**

The Great Reading Adventure is a web application running on Microsoft’s Active Server Pages and .NET frameworks.It uses the SQL Server database software for user and program management along with the IIS Web Server for production and access. For the purposes of this document, it will be assumed that your database server and web server are separate with different IPs. However this is not a necessity and you can run the system in a single server environment. You will need to modify your settings as needed for a single server setup.

To illustrate the installation of the Great Reading Adventure, this document will demonstrate the setup of a training environment for your staff. We recommend setting up two versions of the software, one for training and the other for production. For security, make sure you set each with their own passwords and users in the database server. The training version will allow you to teach others how the software works without the possibility of altering live data.

We’ll cover both sides of the installation beginning with the database side. The web side of the program cannot be installed without pre-existing database linkage, so it’s recommended that the database be set up first.

***

### Database Setup

Setting up the database for the Great Reading Adventure is a two part operation. The first part involves the creation of the database and the second part covers the creation of users for the database and assigning their roles and permissions.

## Creating the Database

1. On your database server, launch Microsoft SQL Server Management Studio and log in as needed. You will need administrative rights to create the database and users so make sure the login has the proper permissions granted to the user. Within the Object Explorer, right click the main Databases folder and select *New Database*.
![New Database Screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DB1_zpszu8pgu76.png)

2. Give the new database a unique, but easily identifiable, name. Depending on your database server’s setup and procedures, make any changes to saving data and log files as you need.
![Database Name Screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DB2_zpshrxqt7qe.png)

3. Under Options set your Recovery model to simple. You can use a more advanced recovery model if you like, but this works well with the software. Click the OK button and save the database.
![Recovery Model Screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DB3_zpsbpbszxud.png)

The new database is ready. Now we’ll create the users for this database.

## Database Users

1. In the Object Explorer, expand your Security folder and right click on _Logins_. Select _New Login_.
![New Login Screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DBU1_zpssi1r2qlo.png)

2. We’ll create the runtime user first. This will be the login that handles data processing after the program is installed. Give the database user a unique but identifiable login name. They will need to authenticate through SQL and you may assign any password you choose. Do not set for password expiration. Finally, set their _Default database_ to the one you just created.

    ![Default database screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DBU2_zpsckkecxdm.png)

    In this case, we’re calling them _stafftrainuser_.

3. Under _User Mapping_, map the user to the database you created and assign _dbo_ as their _Default Schema_. Set their roles to public and db_owner. Click OK.

    ![User mapping screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DBU5_zpsxqam0ypu.png)

4. Now we’ll create the _dbo_ user. This user provides initial setup of the database from the web side of Great Reading Adventure. For security, give them a different password than your user, but allow for SQL Server authentication and set the _Default database_ to the one you created for the system.

    ![dbo user screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DBU4_zpsbhq6mg0n.png)

5. Under _User Mapping_, map the new user to the database with a _Default Schema_ of _dbo_. Set their roles to _public_ and _db_owner_.

    ![User mapping screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DBU5_zpsxqam0ypu.png)

6. You’ll need to execute a SQL Query upon the runtime user, which was stafftrainuser. The query is:
**GRANT EXECUTE ON SCHEMA :: dbo TO stafftrainuser WITH GRANT OPTION;**
Replace “stafftrainuser” with the name of the user for your database. Execute the query, making sure you select the proper database for execution.

    ![SQL query screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DBU6_zps3ij5z8vv.png)

7. If all is well, your SQL Query should complete successfully.

     ![Success message screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/DBU7_zpsaou1fzdw.png)

**Congratulations!**
Database set up is complete! Note down the database name, the user names, and the passwords you assigned to the user names. You will need to know those for the web side installation and setup.


***


### Web Server Setup


Setting up the web server is easier than the database setup and the process is comparable to setting up other web based applications such as WordPress or Coppermine.

1. Extract the contents of the GRA zip file and copy them to a folder in the directory on your webserver from which you publish to the web. On servers running Internet Information Services (IIS), this is usually C:\inetpub\wwwroot. For this example, we’ve created a folder called gra2 and so our files will be copied to C:\inetpub\wwwroot\gra2. Make sure that IUSR and IIS_IUSRS have full permissions on this folder. To do this,  right click the folder, select the "Security" tab, and click "Edit." From the resulting screen, click "Add" and search for IUSR. Click "Add" again and click "Locations" Choose your highest level server from the location list, and click "OK." Then, search for IIS_IUSRS and click "OK." Make sure both of these users have full permissions on the folder.

    ![Extract to webserver screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/WS1_zpsvhm8vzin.png)

2. Open IIS Manager and add a new website by right-clicking the Sites folder and selecting Add Web Site...

    ![Add website screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/WS2_zps3dksap58.png)

3. Give the site a name, in this case we’re calling it gra02. For the Application Pool, select ASP.NET v4.0 Classic. Under "Content Directory," choose the physical path of the folder you created in Step 1.

    ![Name site screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/WS3_zpsp99bovjn.png)

4. Set your bindings as you need to and as desired. If you’re running the site on a IIS web server with multiple sites, then you’ll need to set a host header to a URL used by the system. Set to Type: http on all unassigned IP addresses answering on Port 80. Start the website immediately. For this ex-ample we’re setting the host name to summer.mylibrary.org. Click OK.

    ![Set bindings screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/WS4_zpspe3mlfdu.png)

5. Configure your email options so that email is delivered to an SMTP server or stored in a pickup directory.

    ![SMTP configuration screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/email_zps1qbqmq9b.png)

6. Now, open a browser and go to:
http://your_domain/ControlRoom/Setup.aspx

In our example, we’d go to http://summer.mylibrary.org/ControlRoom/Setup.aspx

Fill out the form with the IP of your data-base server and the name of your data-base. The SA Username and pass are for the dbo user. Use the runtime user and pass in the Runtime Username fields. If you’re attaching a mailserver to this webapp, put in the IP number for that and click Install.


   ![Database setup screenshot] (http://i288.photobucket.com/albums/ll161/greatreadingadventure/WS5_zpsftbvi3rd.png)


## Now let’s get things finalized.

Go to: http://your_domain/ControlRoom
<br>In our example we’d go to: http://summer.mylibrary.org/ControlRoom

<br>**Log in with:**
<br>User Name: _sysadmin_
<br>Password: _changeme05!_

![Control Room screenshot](http://i288.photobucket.com/albums/ll161/greatreadingadventure/WS6_zpsivgpq3wu.png)

You’ll be prompted to change that password when you log in. Once you do, congratulations!
You are done with the installation process!
