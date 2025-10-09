# Upgrading

**There is no upgrade path from versions 2 or 3 to this release** due to significant architectural changes. Follow these instructions to upgrade from a previous release of version 4. When upgrading from a version prior to 4.5 note that you'll need to ensure you have the appropriate version of the [ASP.NET Core Hosting Bundle](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-8.0) on the server.

Note that during the upgrade there will be an interruption in service so it may be ideal to schedule this upgrade in off hours.

## Upgrading a docker instance

1. Get the name of the current Docker container (`docker ps -a`). We'll assume it's named `gra` for this example.
2. Stop the current Docker container (`docker stop gra`).
3. Remove the current Docker container (`docker rm gra`). Data will be saved in your database and any uploaded files are contained in your `shared` directory.
4. Look for any GRA docker images you have on your system (`docker image ls`).
5. Remove any GRA docker images on your system (`docker rmi mcld/gra:latest`).
6. Use the command that you initially used to run the Docker container to download the latest container and run it (for example `docker run -d -p 80:80 --name gra --restart unless-stopped -v /gra/shared:/app/shared mcld/gra`, see the [Install the Software](../installation/install-the-software) section of this manual for more information).
7. Load the site in your browser. It will take longer than normal as the database is upgraded (you can see evidence of the database upgrade in the log file in `shared/logs`).

## Upgrading a Web server

1. Back up the database and the files that comprise the Web site.
2. If you have modified any files in the application (such as `.cshtml` Razor template files), please make a note of which changes you have made, these changes will have to be performed again. Changes to the `shared` directory (such as to `style.css` or uploaded files) will be maintained.
3. Replace the application files with the files from this release. **Ensure that your `shared` directory is not overwritten when replacing application files**.
4. The `appsettings.json` file in the application folder will be replaced when you copy in files from the release. If you modified this file please compare to the file you backed up in the first step above to ensure any configuration changes you made initially are set in the new file. If you added an `appsettings.json` file in the `shared` directory with your settings it will **not be overwritten** and you shouldn't need to change any settings.
5. Load the site in your browser. It will take longer than normal as the database is upgraded (you can see evidence of the database upgrade in the log file in `shared/logs`).
