Technical configuration details
===============================

Web.config application settings
-------------------------------

These settings are in the ``Web.config`` file in the ``<appSettings>`` section. A typical configuration might look like this:

.. code-block:: xml

  <appSettings>
    <add key="LogEmails" value="True" />
  </appSettings>

.. DANGER::
   Changing values in the ``Web.config`` will cause the Web application to be restarted causing any current sessions to be reset. Avoid changing the ``Web.config`` when people are using the site!

The following is a list of keys and what the settings represent:

**LogEmails** (``True`` or ``False``) - log all sent emails in the database

**UseEmailTemplates** (``True`` or ``False``) - use HTML email templates

**DefaultEmailTemplate** (path to HTML file) - path to the HTML file to use as the email template if ``UseMailTemplates`` is enabled

**DefaultEmailFrom** (email address) - default email from address if one is not set in the software

**IgnoreMissingDatabaseGroups** (``True`` or ``False``, optional) - whether the configuration process will ignore checks on if the database users are in the ``db_owner``, ``db_datareader``, and ``db_datawriter`` SQL groups - normally if the database users are not in the appropriate groups it is presented as an error

**ILSProxyEndpoint** (URL to ILSProxy endpoint, optional) - URL to an ILS proxy to be used for looking up patrons by library account

**IsbnLinkTemplate** (URL template, optional) - default URL template when adding books to a Challenge/book list. It should be a URL linking to your ILS by ISBN for users who are not logged in. In the URL, you'll replace the actual ISBN with ``{0}``. For example, using Polaris it would be ``http://pac.url/view.aspx?isbn={0}``.
