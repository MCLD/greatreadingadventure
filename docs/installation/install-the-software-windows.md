# Install the software on a Windows Server

The final installation step is to create a Web site and install the GRA software.

The hosting Windows Server should have [IIS installed and the Web Sockets feature enabled](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-1.1&tabs=aspnetcore1x#iis-configuration).  You do not need to install Windows Authentication.

Also, ensure the server has the proper [.NET Core 1.1.x Hosting Bundle installed](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-1.1&tabs=aspnetcore1x#install-the-net-core-hosting-bundle).

## Set up the GRA as the only site on the server

If this Web server is configured solely for the Great Reading Adventure, you can utilize the default Web site for your GRA installation.

1. Delete existing default files placed in `c:\inetpub\wwwroot\` such as `iisstart.htm` and `welcome.png`.
2. Unzip the GRA files into `c:\inetpub\wwwroot\`. Ensure that the files are placed in that directory directly and not in a subdirectory (you should see files such as `appsettings.json` and `GRA.dll` present).
3. Create a new folder named `shared` in `c:\inetpub\wwwroot`.
3. Right-click on the `c:\inetpub\wwwroot\shared` directory and select **Properties**.
4. Choose the **Security** tab and then click the **Edit** button next to *To change permissions, click Edit*.
5. If you don't see the `IIS_IUSRS` group listed, select **Add** in the **Permissions for wwwroot** window and in the **Enter the object names to select** box, enter in `IIS_IUSRS` and click **OK** (the `IIS_IUSRS` group is a local group on this machine, so ensure that the **From this location** box has the name of the Web server and not your domain - you can change this by clicking **Locations...** and select the server). You may also [use an AppPoolIdentity account instead of the `IIS_IUSRS` group](https://blogs.technet.microsoft.com/tristank/2011/12/22/iusr-vs-application-pool-identity-why-use-either/) for security reasons.
7. Ensure all the checkboxes (including Modify) are selected except **Full control** and **Special permissions** in the **Allow** column.
8. Select **OK** to close this window and **OK** to close the **wwwroot Properties** window.
9. Ensure you have updated the configuration in the `appsettings.json` file - or if you are using a configuration override file, ensure it's named `appsettings.json` and placed into the `shared` folder. See [the configuration section of this document](configuration) for more details.
10. Launch a Web browser on the server and navigate to the URL ``http://localhost/``.
11. At this point you should see the initial GRA setup screen.
12. You can continue the GRA setup process either directly in this Web browser or you can navigate to the Web server name or IP address in a browser on another system to continue.

## Set up the GRA as an additional site on the server

If you are deploying the Great Reading Adventure to a Web server which is already hosting one or more Web sites, you must create a new Web Site specifically for the GRA.

Because this Web server is already serving out files through the default Web Site, you must differentiate your GRA Web site somehow. Methods for doing so include:

* Setting up a separate host name for your GRA Web site (for example: `http://gra.<your domain>`. This method utilizes the [HTTP/1.1. Host Header Field](<http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.23>).
* Setting up a separate IP address on the Web server and putting your GRA site there.
* Setting up the GRA on a different port number so that you'll access it via `http://<your Web server>:<GRA port>/`.

Your system administrator can help you select the correct approach.

1. Create a directory on the Web server to contain the GRA files. For example, `c:\inetpub\gra`.
2. Unzip the GRA files into that directory. Ensure that the files are placed in that directory directly and not in a subdirectory (you should see files such as `appsettings.json` and `GRA.dll` present).
3. Right-click on directory created in step 1 above and select **Properties**.
4. Choose the **Security** tab and then click the **Edit** button next to *To change permissions, click Edit*.
5. If you don't see the `IIS_IUSRS` group listed, select **Add** in the **Permissions for ...** window and in the **Enter the object names to select** box, enter in `IIS_IUSRS` and click **OK** (the `IIS_IUSRS` group is a local group on this machine, so ensure that the **From this location** box has the name of the Web server and not your domain - you can change this by clicking **Locations...** and select the server). You may also [use an AppPoolIdentity account instead of the `IIS_IUSRS` group](https://blogs.technet.microsoft.com/tristank/2011/12/22/iusr-vs-application-pool-identity-why-use-either/) for security reasons.
6. Ensure all the checkboxes (including Modify) are selected except **Full control** and **Special permissions** in the **Allow** column.
7. Select **OK** to close this window and **OK** to close the **Properties** window.
8. Ensure you have updated the configuration in the `appsettings.json` file - or if you are using a configuration override file, ensure it's named `appsettings.json` and placed into the `shared` folder. See [the configuration section of this document](configuration) for more details.
9. Open up the **Internet Information Services Manager** on the Web server.
10. Expand the Server under **Connections**.
11. Right-click on **Sites** and choose **Add Web Site...**.
12. Enter an appropriate site name such as `SummerReading`.
13. Enter the physical path where you put the GRA files under **Physical path** (we used `c:\inetpub\gra` above).
14. In the **Binding** section you will either need to assign an IP address, a host name, or select a different port as defined above.
15. Select **OK** to close the **Add Web Site** window.
16. Launch a Web browser on the server and navigate to the URL you defined for this install of the GRA.
17. At this point you should see the initial GRA setup screen.
18. You can continue the GRA setup process either directly in this Web browser or you can navigate to the Web server using the URL you defined for this install of the GRA.

## Troubleshooting a Windows installation

1. Did you modify the `appsettings.json` or provide an override file in the `settings` directory to configure the database connection and authorization code?
2. Did you install the .NET Core 1.1.x Hosting Bundle?
3. Did you restart IIS after installing it?
4. Are you sure the `shared` directory has write permissions for the process running IIS? Once the application starts, it will create a "logs" directory there so you will know if the Web server can write to this directory once you see the "logs" directory present. Check that "logs" directory to see if there are any errors written out to the log file.
5. If the Web process can write to the `shared` directory, you can edit the `Web.config` file `aspNetCore` tag to set `stdoutLogEnabled="true"` and `stdoutLogFile=".\shared\stdout"`. When you restart IIS it should write a log file starting with "stdout" that you can examine to see if any errors are being written to it.
6. Microsoft's [Troubleshoot ASP.NET Core on IIS](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/troubleshoot?view=aspnetcore-1.1) page will walk you through some typical troubleshooting steps.
