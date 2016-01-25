# Create the database

The GRA uses its own database to store reading program data. This database can be completely segregated from any other databases on the same SQL Server.

There are two options when creating the database: you can execute a script or manually create the database and users using [SQL Server Management Studio (SSMS)](https://msdn.microsoft.com/en-us/library/ms174173.aspx).

### From a script

1. Launch SQL Server Management Studio on the database server.
2. Right-click on the server in the Object Explorer on the left and choose **New Query**.
3. Copy the text from the [database create script](https://raw.githubusercontent.com/MCLD/greatreadingadventure/master/CreateDatabase.sql) and paste it into the query window.
4. Click the **Execute** button in the toolbar.

### From the user interface

#### Create the database
1. Launch SQL Server Management Studio on the database server.
2. Double-click the server name to show the **Databases** folder.
3. Right-click on the **Databases** folder and select the **New Database...** option.
4. In the New Database window, enter a database name, for example: `SRP`.
5. If you wish to have the database files stored in a [different location than the default](https://support.microsoft.com/en-us/kb/2033523), scroll the **Database files** list to the right and change the presented **Path** to where you'd like to store the database files.
6. Select **Options** from the page list on the left.
7. For the [**Recovery model** drop-down](https://msdn.microsoft.com/en-us/library/ms189275.aspx), change the selection from `Full` to `Simple`.
8. Select **OK** to create the database.

#### Create the logins and users

1. Right-click the **Security** folder and select **New** **->** **Login...**.
2. Enter the **Login name** for the database owner user, for example: `srp_owner`.
3. Select `SQL Server authentication`.
4. Enter a secure password for this user in the **Password** and **Confirm password** fields. Make a note of this password.
5. You can uncheck `Enforce password policy` so that the password for this account will not expire.
6. Select `User mapping` from the **Select a page** list on the left.
7. Locate the name of the database (this tutorial used `SRP` in *Create the database* step 4 above) in the **Users mapped to this login** list on the left.
8. Check the appropriate box in the `Map` column to map this login to a user in the database.
9. In the **Database role membership for:** list, select `db_owner`.
10. Select **OK** to create the login and user.
11. Right-click the **Security** folder and select **New** **->** **Login...**.
12. Enter the **Login name** for the database user, for example: `srp_user`.
13. Select `SQL Server authentication`.
14. Enter a secure password for this user in the **Password** and **Confirm password** fields. Use a different password than you used in *Create the logins and users* step 4 above but make a note of this password as well.
15. You can uncheck `Enforce password policy` so that the password for this account will not expire.
16. Select `User mapping` from the **Select a page** list on the left.
17. Locate the name of the database (this tutorial used `SRP` in *Create the database* step 4 above) in the **Users mapped to this login** list on the left.
18. Check the appropriate box in the `Map` column to map this login to a user in the database.
19. In the **Database role membership for:** list, select `db_datareader` and `db_datawriter`.
20. Select **OK** to create the login and user.

#### Grant execute permissions to the database user

1. Right-click on the database you created (this tutorial used `SRP` in *Create the database* step 4 above).
2. Select **New Query**.
3. In the query window, enter `GRANT EXECUTE ON SCHEMA::dbo TO [srp_user] WITH GRANT OPTION;` (if you used a different database user name in step 10 above you'll need to put that user in the braces `[]` of the query).
4. Click the **Execute** button (or press `F5`).
