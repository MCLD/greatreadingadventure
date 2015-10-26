# Install the software

The final installation step is to create a Web site and install the GRA software.

## Set up the GRA as the only site on the server

If this Web server is configured solely for the Great Reading Adventure, you can utilize the default Web site for your GRA installation.

1. Delete existing default files placed in `c:\inetpub\wwwroot\` such as `iisstart.htm` and `welcome.png`.
2. Unzip the GRA files into `c:\inetpub\wwwroot\`. Ensure that the files are placed in that directory directly and not in a subdirectory (you should see files such as `Web.config` present).
3. Right-click on the `c:\inetpub\wwwroot\` directory and select **Properties**.
4. Choose the **Security** tab and then click the **Edit** button next to *To change permissions, click Edit*.
5. Select **Add** in the **Permissions for wwwroot** window.
6. In the **Enter the object names to select** box, enter in `IIS_IUSRS` and click **OK** (the `IIS_IUSRS` group is a local group on this machine, so ensure that the **From this location** box has the name of the Web server and not your domain - you can change this by clicking **Locations...** and select the server).
7. Select the check box next to **Full control** in the **Allow** column.
8. Select **OK** to close this window and **OK** to close the **wwwroot Properties** window.
9. Open up the **Internet Information Services Manager** on the Web server.
10. Expand the Server under **Connections**, expand the list of **Sites**, and select **Default Web Site**.
11. On the right, under **Actions**, select **Basic Settings...**.
12. Click **Select...** to change the **Application pool** to **ASP.NET v4.0**.
13. Select **OK** to close the **Select Application Pool** window and then **OK** to close the **Edit Site** window.
14. Launch a Web browser on the server and navigate to the URL ``http://localhost/``.
15. At this point you should see the initial GRA setup screen.
16. You can continue the GRA setup process either directly in this Web browser or you can navigate to the Web server name or IP address in a browser on another system to continue.

## Set up the GRA as an additional site on the server

If you are deploying the Great Reading Adventure to a Web server which is already hosting one or more Web sites, you must create a new Web Site specifically for the GRA.

Because this Web server is already serving out files through the default Web Site, you must differentiate your GRA Web site somehow. Methods for doing so include:

* Setting up a separate host name for your GRA Web site (for example: `http://gra.<your domain>`. This method utilizes the [HTTP/1.1. Host Header Field](<http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.23>).
* Setting up a separate IP address on the Web server and putting your GRA site there.
* Setting up the GRA on a different port number so that you'll access it via `http://<your Web server>:<GRA port>/`.

Your system administrator can help you select the correct approach.

1. Create a directory on the Web server to contain the GRA files. For example, `c:\inetpub\gra`.
2. Unzip the GRA files into that directory. Ensure that the files are placed in that directory directly and not in a subdirectory (you should see files such as `Web.config` present).
3. Right-click on directory created in step 1 above and select **Properties**.
4. Choose the **Security** tab and then click the **Edit** button next to *To change permissions, click Edit*.
5. Select **Add** in the **Permissions for ...** window.
6. In the **Enter the object names to select** box, enter in `IIS_IUSRS` and click **OK** (the `IIS_IUSRS` group is a local group on this machine, so ensure that the **From this location** box has the name of the Web server and not your domain - you can change this by clicking **Locations...** and select the server).
7. Select the check box next to **Full control** in the **Allow** column.
8. Select **OK** to close this window and **OK** to close the **Properties** window.
9. Open up the **Internet Information Services Manager** on the Web server.
10. Expand the Server under **Connections**.
11. Right-click on **Sites** and choose **Add Web Site...**.
12. Enter an appropriate site name such as `SummerReading`.
13. Enter the physical path where you put the GRA files under **Physical path** (we used `c:\inetpub\gra` above).
14. In the **Binding** section you will either need to assign an IP address, a host name, or select a different port as defined above.
15. Select **OK** to close the **Add Web Site** window.
16. On the left, in **Connections**, select **Application Pools**.
17. Find the Application Pool in the list named the same as your site (the name selected in step 12 above).
18. Double-click the appropriate Application Pool and ensure the **.NET Framework version** is set to `.NET Framework v4.0.30319` or newer.
19. Ensure the **Managed pipeline mode** is set to `Integrated`.
20. Select **OK** to close the **Edit Application Pool** window.
21. Launch a Web browser on the server and navigate to the URL you defined for this install of the GRA.
22. At this point you should see the initial GRA setup screen.
23. You can continue the GRA setup process either directly in this Web browser or you can navigate to the Web server using the URL you defined for this install of the GRA.