# Requirements checklist

- Ensure you have a Windows server running **Windows Server 2008 or newer**
- Ensure your server has the **.NET Framework version 4.0 or newer installed**
- Confirm that you can **create a new Web site in IIS** on this server
- Confirm that you will be permitted to configure it so that **Web site files can be writable by the Windows user who owns the IIS process (typically the `IIS_IUSRS` group)**.
- Ensure you have access to a **Microsoft SQL Server version 2008 or newer**
- Ensure that you'll be able to authenticate in **SQL Server authentication mode**
- Confirm that you'll be able to **create a database**
- Ensure that you have a **mail server with an accessible SMTP port or the ability to deliver mail from a service running on the Web server**
- The **[latest release of the GRA software](https://github.com/MCLD/greatreadingadventure/releases/latest)** downloaded from GitHub.

## Configuration information you'll need

### Configuration step 1: database configuration
- Database server name or IP address
- Database/catalog name
- Database owner user login (the user in the `db_owner` role)
- Database owner user password
- Database regular user login (the user in the `db_datareader` and `db_datawriter` roles)
- Database regular user password

### Configuration step 2: mail server configuration
- The administrator's email address - you may want to set up a role address ahead of time so that system emails don't appear to come from your personal address
- Mail server (optional) - SMTP server to handle emails. If you do not specify one, it will leave them on the server (in `c:\inetpub\smtp\` by default) to be picked up by mail delivery software
- Mail server port (optional) - by default 25 will be used
- Mail server login (optional) - if you need to authenticate to send email
- Mail server password (optional)

### Configuration step 3: select an initial program configuration

Your final decision is which initial program configuration to choose:

- You can opt to set up with a single reading program
- You can opt to set up with four age-specific reading programs.

**For more information on these options, please review the [planning section](../../introduction/planning) of this manual.**