## Page/control philosophy

Originally the code was deployed as pages and controls without any clear demarcation. During UX improvements, we've tried to set out an architecture of `.aspx` pages having a single top-level Bootstrap layout (the core of which is defined in `~/Layout/SRP.Master`) which `.ascx` user controls are placed into. Each `.ascx` user control provides its own Bootstrap layout. This should make it easy to move controls around on pages.

`Dashboard.aspx` provides an example of a three-column Bootstrap grid page that contains multiple `.ascx` controls.

## Visual Studio Projects/Namespaces

Legacy code is gradually being refactored into a more logical project layout with more appropriate namespacing.

| Project            | Value
| ------------------ | -------
| GRA.Communications | Handle communications external to the GRA, currently only email.
| GRA.Database       | [Database project](https://msdn.microsoft.com/library/xee70aty(v=vs.100).aspx) containing the database schema
| GRA.Logic          | Logic reused throughout the GRA.
| GRA.Tools          | Helpers and tools - coding shortcuts and non-subject-matter logic (e.g. logging wrappers, password hashing)
| SQLHelper          | The retired [Microsoft Data Access Application Block for .NET](https://msdn.microsoft.com/en-us/library/ff649538.aspx) which is currently the way the DAL queries the database
| SRP                | The actual Web site
| SRP_DAL            | [Data access layer](https://en.wikipedia.org/wiki/Data_access_layer) for providing database access and logic to code.
| SRPUtilities       | Legacy code utilities and tools, probably eventually refactored into `GRA.Logic` or `GRA.Tools`

## Conventions

### Code
These are made as a best-effort approach. Much of the legacy code does not follow these conventions.

- Try to follow the [HTML](http://codeguide.co/#html) and [CSS](http://codeguide.co/#css) code guides.
- Wrap lines at 100 characters whenever possible.
- `IDisposable` calls wrapped in `using()`.
- Follow [.NET Framework Guidelines and Best Practices](https://msdn.microsoft.com/library/ms184412%28v=vs.100%29.aspx?f=255&MSPPError=-2147217396) as much as possible.

### Avoid magic numbers

Magic numbers should typically be placed in clearly-named constants. As an example, keys used to store values in system dictionaries: `Session` keys should use constants in `GRA.Tools.SessionKey`, `ViewState` keys should use local constant strings.

### `Web.config` is for system settings

User preferences belong somewhere that they can be changed inside the application. Appropriate values for the `Web.config` include:

- External system access information (`connectionStrings`, `mailSettings`)
- System default values (`DefaultEmailTemplate`, `DefaultEmailFrom`)
- Logging configuration (NLog, ELMAH)
- External API access information (`ILSProxyEndpoint`, `IsbnLinkTemplate`)

`Web.config` settings (especially `appSettings`) should be documented in the [appropriate section of the manual](http://manual.greatreadingadventure.com/en/latest/technical/configuration-details/).

## Sending messages to a patron

To send a text message to the user, the `GRA.SRP.SessionTools` helper can be used. The constructor takes the current `Session` and pushes variables there which are then picked up in the master page (`~/Layout/SRP.Master`) to be shown to the patron. The master page also handles removing the Session variables used to pass the message (also using `GRA.SRP.SessionTools`).

### Text message

To send a regular text message to the patron use `GRA.SRP.SessionTools.AlertPatron`:

| Parameter          | Value
| ------------------ | -------
| message            | Text to show the user in a [Bootstrap alert](http://getbootstrap.com/components/#alerts) at the top of the page
| patronMessageLevel | The message level, use the `GRA.Tools.PatronMessageLevels` static class. Optional, defaults to success.
| glyphicon          | The name of the [Glyphicon](http://getbootstrap.com/components/#glyphicons) to show in the alert - just the name after the `glyphicon-` will do, for example `"asterisk"` or `"plus"`. Optional.

For message level and glyphicon guidance, review the [Bootstrap conventions](BootstrapConventions.md).

### Earned badge pop-up

To send a message about earned badges, use `GRA.SRP.SessionTools.EarnedBadges` by passing it the result of the badge earning trigger (the result from `GRA.SRP.Controls.AwardPoints` which is a string of pipe-separated badge IDs).
