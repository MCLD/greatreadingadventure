# Upgrading

**There is no upgrade path from versions 2 or 3 to this release** due to significant architectural changes. Follow these instructions to upgrade from a previous release of version 4 (i.e. 4.0.0-beta1 and 4.0.0-beta2).

Note that during the upgrade there will be an interruption in service so it may be ideal to schedule this upgrade in off hours. To upgrade from prior versions follow these steps:

1. Back up the database and the files that comprise the Web site.
2. If you have modified any files in the application (such as `.cshtml` Razor template files), please make a note of which changes you&rsquo;ve made, these changes will have to be performed again. Changes to the `shared` directory (such as to `style.css` or uploaded files) will be maintained.
3. Replace the application files with the files from this release. **Ensure that your `shared` directory is not overwritten when replacing application files**.
4. The `appsettings.json` file in the application folder will be replaced when you copy in files from the release. If you modified this file please compare to the file you backed up in the first step above to ensure any configuration changes you made initially are set in the new file. If you added an `appsettings.json` file in the `shared` directory with your settings it will **not be overwritten** and you shouldn&rsquo;t need to change any settings.
5. Load the site. It will take longer than normal as the database is upgraded (you can see evidence of the database upgrade in the log file in `shared/logs`).
