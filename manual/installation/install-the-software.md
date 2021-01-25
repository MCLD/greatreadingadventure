# Install the software

The final installation step is to install and run the GRA software. We recommend deploying the GRA in Docker.

## Install and run the GRA in Docker

The GRA Docker image requires a Linux container, if you are running Docker on a platform other than Linux please ensure that you have Linux containers selected. If you don't currently have a Web site running on your Docker server you can forward port 80 from the server directly into the Docker container.

1. Create a directory on the Web server to contain the shared GRA files, for example `/gra/shared` in Linux or `c:\gra\shared\` in Windows.
2. Place your `appsettings.json` file into the shared directory that you just created.
3. From a prompt (a `bash` shell, `command prompt` or `PowerShell` window) start the Docker image with a command similar to `docker run -d -p 80:80 --name gra --restart unless-stopped -v /gra/shared:/app/shared mcld/gra` - details of that command:
   - `-d` tells Docker to run the container in the background
   - `-p 80:80` says to forward port 80 from the local server to port 80 in the container
   - `--name gra` provides a name for the container to make it easier to reference while it's running (e.g. if you have to stop the container you can with `docker stop gra`)
   - `--restart unless-stopped` will restart the container if it should stop unless you explicitly stop it
   - `-v /gra/shared:/app/shared` tells Docker to share the `/gra/shared` directory on the local server with the `/app/shared` directory inside the container
   - `mcld/gra` is the image to run - this will download and run the `mcld/gra:latest` image from [Docker Hub](https://hub.docker.com/r/mcld/gra/)
4. Launch a Web browser on the server and navigate to the URL you defined for this install of the GRA (for the above example, `http://localhost/` will work).
5. At this point you should see the initial GRA setup screen.
6. You can continue the GRA setup process either directly in this Web browser or you can navigate to the Web server using the URL you defined for this install of the GRA.

In the case that there is already a Web site running on your server you will need to forward a different port into the Docker container. If you chose to forward port 2001 from your server into the GRA container you'd use `-p 2001:80` in step 3 above and then in step 4 you'd access the site by navigating to `http://localhost:2001/`.

The software should create a "logs" directory inside the `shared` directory which you can review to see if there are any errors written out to the log file.

## Install and run the GRA on a Windows Server

The GRA is an ASP.NET Core application so review the [Host ASP.NET Core on Windows with IIS
](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-5.0) instructions from Microsoft and ensure that IIS is installed with the Web Sockets feature enabled and the appropriate ASP.NET Core hosting bundle is installed.

### Set up the GRA as the only site on the server

If this Web server is configured solely for the Great Reading Adventure, you can utilize the default Web site for your GRA installation.

1. Delete existing default files placed in `c:\inetpub\wwwroot\` such as `iisstart.htm` and `welcome.png`.
2. Unzip the GRA files into `c:\inetpub\wwwroot\`. Ensure that the files are placed in that directory directly and not in a subdirectory (you should see files such as `appsettings.json` and `GRA.dll` in `c:\inetpub\wwwroot\`).
3. Create a new folder named `shared` in `c:\inetpub\wwwroot`.
4. Right-click on the `c:\inetpub\wwwroot\shared` directory and select **Properties**.
5. Choose the **Security** tab and then click the **Edit** button next to _To change permissions, click Edit_.
6. If you are using IIS 7.5 or later (it shipped starting with Windows 7 and Windows Server 2008 R2), select **Add** in the **Permissions for wwwroot** window and in the **Enter the object names to select** box, enter in `IIS AppPool\DefaultAppPool` and click **OK** (the `IIS AppPool\DefaultAppPool` user is a local user on this machine, so ensure that the **From this location** box has the name of the Web server and not your domain - you can change this by clicking **Locations...** and select the server). You may also [potentially use the `IIS_IUSRS` group](https://blogs.technet.microsoft.com/tristank/2011/12/22/iusr-vs-application-pool-identity-why-use-either/) if you prefer.
7. Ensure all the checkboxes (including Modify) are selected except **Full control** and **Special permissions** in the **Allow** column.
8. Select **OK** to close this window and **OK** to close the **wwwroot Properties** window.
9. Ensure you have updated the configuration in the `appsettings.json` file - or if you are using a configuration override file, ensure it's named `appsettings.json` and placed into the `shared` folder. See [the configuration section of this document](configuration) for more details.
10. Launch a Web browser on the server and navigate to the URL `http://localhost/`.
11. At this point you should see the initial GRA setup screen.
12. You can continue the GRA setup process either directly in this Web browser or you can navigate to the Web server name or IP address in a browser on another system to continue.

### Set up the GRA as an additional site on the server

If you are deploying the Great Reading Adventure to a Web server which is already hosting one or more Web sites, you must create a new Web Site specifically for the GRA.

Because this Web server is already serving out files through the default Web Site, you must differentiate your GRA Web site somehow. Methods for doing so include:

- Setting up a separate host name for your GRA Web site (for example: `http://gra.<your domain>`. This method utilizes the [HTTP Host Header Field](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.23).
- Setting up a separate IP address on the Web server and putting your GRA site there.
- Setting up the GRA on a different port number so that you'll access it via `http://<your Web server>:<GRA port>/`.

Your system administrator can help you select the correct approach.

1. Create a directory on the Web server to contain the GRA files. For example, `c:\inetpub\gra\`.
2. Unzip the GRA files into that directory. Ensure that the files are placed in that directory directly and not in a subdirectory (you should see files such as `appsettings.json` and `GRA.dll` in `c:\inetpub\gra\`).
3. Create a new folder named `shared` in the directory you created above.
4. Right-click on the `shared` directory and select **Properties**.
5. Choose the **Security** tab and then click the **Edit** button next to _To change permissions, click Edit_.
6. If you are using IIS 7.5 or later (it shipped starting with Windows 7 and Windows Server 2008 R2), select **Add** in the **Permissions for wwwroot** window and in the **Enter the object names to select** box, enter in `IIS AppPool\DefaultAppPool` (or the name of the App Pool into which you are deploying) and click **OK** (the `IIS AppPool\DefaultAppPool` user is a local user on this machine, so ensure that the **From this location** box has the name of the Web server and not your domain - you can change this by clicking **Locations...** and select the server). You may also [potentially use the `IIS_IUSRS` group](https://blogs.technet.microsoft.com/tristank/2011/12/22/iusr-vs-application-pool-identity-why-use-either/) if you prefer.
7. Ensure all the checkboxes (including Modify) are selected except **Full control** and **Special permissions** in the **Allow** column.
8. Select **OK** to close this window and **OK** to close the **Properties** window.
9. Ensure you have updated the configuration in the `appsettings.json` file - or if you are using a configuration override file, ensure it's named `appsettings.json` and placed into the `shared` folder. See [the configuration section of this document](configuration) for more details.
10. Open up the **Internet Information Services Manager** on the Web server.
11. Expand the Server under **Connections**.
12. Right-click on **Sites** and choose **Add Web Site...**.
13. Enter an appropriate site name such as `SummerReading`.
14. Enter the physical path where you put the GRA files under **Physical path** (we used `c:\inetpub\gra` above).
15. In the **Binding** section you will either need to assign an IP address, a host name, or select a different port as defined above.
16. Select **OK** to close the **Add Web Site** window.
17. Launch a Web browser on the server and navigate to the URL you defined for this install of the GRA.
18. At this point you should see the initial GRA setup screen.
19. You can continue the GRA setup process either directly in this Web browser or you can navigate to the Web server using the URL you defined for this install of the GRA.

### Troubleshooting a Windows installation

1. Did you modify the `appsettings.json` or provide an override file in the `settings` directory to configure the database connection and authorization code?
2. Did you install the ASP.NET Core Hosting Bundle?
3. Did you restart IIS after installing it?
4. Are you sure the `shared` directory has write permissions for the process running IIS? Once the application starts, it will create a "logs" directory there so you will know if the Web server can write to this directory once you see the "logs" directory present. Check that "logs" directory to see if there are any errors written out to the log file.
5. If the Web process can write to the `shared` directory, you can edit the `Web.config` file `aspNetCore` tag to set `stdoutLogEnabled="true"` and `stdoutLogFile=".\shared\stdout"`. When you restart IIS it should write a log file starting with "stdout" that you can examine to see if any errors are being written to it.
6. Microsoft's [Troubleshoot ASP.NET Core on IIS](https://docs.microsoft.com/en-us/aspnet/core/test/troubleshoot-azure-iis?view=aspnetcore-5.0) page will walk you through some typical troubleshooting steps.
