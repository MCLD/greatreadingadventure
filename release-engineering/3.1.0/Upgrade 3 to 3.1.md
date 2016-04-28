**Version 3.1.0 of the [Great Reading Adventure](http://greatreadingadventure.com/) was released on April 27th, 2016**. It fixes a number of issues and includes a ton of enhancements. You can view the [release notes](https://github.com/MCLD/greatreadingadventure/releases/tag/v3.1.0) at GitHub.

If your current installation does not have a ton of content you are probably going to find it easier to just do a clean install from scratch. This upgrade process involves multiple steps including replacing files and running a database update script.

**This process has been tested to the best of our ability but there is always a chance that we have missed something or that our environment is not the same as yours. Please, please, *please* take backups of the GRA files and the database prior to taking any steps after number 6 below!**

As part of this update process, your existing avatars will no longer function. The avatar system has been upgraded and features customizable avatars. Bear this in mind if you've customized your avatar artwork.

Here are the steps required to perform the upgrade:

1. Download and unzip the [`GRA-v3-to-v3.1-upgrade.zip`](https://github.com/MCLD/greatreadingadventure/releases/tag/v3.1.0) file.

2. Locate your current GRA installation. The folder will contain files such as `Web.config` and `Dashboard.aspx`.

3. Create a backup of the files in your current GRA installation. The easiest approach is to select all the files, right-click, and choose `Send To -> Compressed (zipped) Folder`.

4. Copy that backup file somewhere safe. If something goes wrong in the upgrade you will want to restore these files!

5. Obtain a connection to your database so that you can back up your current database and run SQL commands or scripts. The approach for this will vary based on the way your GRA is hosted, this may require running [SQL Server Management Studio](https://msdn.microsoft.com/en-us/library/ms174173.aspx) or using your hosting provider's database interface (usually through some sort of Web dashboard).

6. Back up your current database and ensure the backup file is placed somewhere safe. Again, this process will vary depending upon your configuration and whether or not you are using [SQL Server Management Studio to perform the backup](https://msdn.microsoft.com/en-us/library/ms187510.aspx#Anchor_1).

  *__NOTE: from this step on, program patrons and Control Room users may see an interruption in service as we progress through the upgrade - consider making this change at a time when few users are accessing the site.__*

7. Copy the files from the `GRA Upgrade` folder into the folder of your current GRA installation. This will overwrite most of the files and folders.

  **NOTE:** If you have customized your favicon/site shortcut icons note that they'll be overwritten with the Great Reading Adventure logo icons.

8. Modify the existing `Web.config` file - change the line that says:

    `<pages validateRequest = "false"/>`

    To say:

    `<pages validateRequest="false" enableEventValidation="false"/>`

9. Open the `images` folder in your GRA installation.

10. Create two new folders: `AvatarParts` and `AvatarCache`.

11. Copy the files in the `GRA Upgrade AvatarParts` folder into the newly-made `AvatarParts` folder in your GRA installation.

12. Copy the files in the `GRA Upgrade AvatarCache` folder into the newly-made `AvatarCache` folder in your GRA installation.

13. Execute the `GRA 3 to 3.1.sql` database update script against your GRA database. This script is a normal text file you can open in Notepad.

14. Log in to the Control Room and verify that you are able to navigate without seeing any errors. We'd suggest at least checking the `Patrons` tab (search and view a Patron's profile) and the `Management` tab (view some content like Badges and Challenges).

15. Navigate to the `Programs` tab.

16. Open the `GRA Upgrade Resource Text.txt` file in the upgrade folder. This is a normal text file that you can open in Notepad.

17. Repeat the following steps for each of your programs:

  a. Click the `Edit Record` icon next to your program.

  b. Select `Static Text` at the top of the screen.

  c. Scroll down to the bottom of the Program Text Resources (the large text field).

  d. Copy the contents of the `GRA Upgrade Resource Text.txt` to the end of the text in the text field.

  e. Click `Save Program Text Resources`.

  f. Use the `Program List` link at the top of the screen to navigate back to the `Programs` tab to repeat this process.

18. If your program is available for registration, log out of the Control Room and log into the patron side to verify that everything is working.

19. (Optional) customize the avatar of your patron account to verify the Avatar customization is working properly.

20. After these steps have been taken, you have successfully upgraded your Great Reading Adventure installation. If you have further questions, please post to [the forum](http://forum.greatreadingadventure.com/c/howto) and we will help you sort it out!
