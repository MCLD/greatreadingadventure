# Site settings

By default site settings coded into the software or provided in the configuration file are used to define things like the name of the site and the page footer. Once you can access Mission Control (using the rocket icon in the navigation bar), these can all be customized by selecting the wrench icon on the right side of the navigation bar and choosing "Site management" from the drop-down menu.

## Details

Please customize these public-facing details to be relevant to your program.

- **Name** - How the site will be referred to when it's referenced in text in the templates. For example "Authorization code for `name`" or "Thanks for participating in `name`".
- **Path** - Planned for use with multitenancy. Isn't displayed and doesn't need to be changed.
- **Page Title** - Used throughout the site in the `<title>` tag of the page which is seen by both participants and search engines.
- **Meta Description** - The contents of the `<meta description="...">` tag on the front page of the site for users who are not logged in. This often what is displayed as a description for your site on search engines.
- **Footer** - Footer shown on every page of the site. Unlike the other options on this page you can enter the footer using the [CommonMark text markup language](https://commonmark.org/) (which the editor buttons above the field can assist with if you're not familiar with CommonMark). If you need more flexability you can enter HTML directly into this field. The CommonMark and/or HTML will be rendered below the box in the **Footer Preview** area in real time with the selected paragraph highlighted with a light grey background.

Ensure you click **Save** after making changes on this page!

## Configuration

This tab contains settings necessary for the system to run properly.

- **Site Logo URL** - An image shown on administration screens, generally not viewable by the public. It is okay to leave this with the system default.
- **External Event List URL** - Configuring a URL here will link to that site rather than use the built-in event system.
- **Max Points Per Challenge Task** - The maximum number of points that should be allowed to be configured for each Challenge task.
- **Single Page Sign Up** - Allow users to sign up on a single page rather than stepping through several pages. If you only require a few bits of information for a participant to sign up you may prefer the simpler single-page process. If you are prompting for a lot of sign-up information (such as schools) it may be overwhelming to have all of the sign up fields on a single page.
- **Require Postal Code** - Require a postal code for participants to sign up.
- **Google Analytics Tracking Id** - The GRA includes the [Google Tag Manager include file](https://developers.google.com/analytics/devguides/collection/gtagjs/) on every page of the site if this field is configured with a Google tracking id (which is usually similar to `UA-########-#`). Please note: if you are using Google Analytics, see below for configuring the two additional dimensions which are sent as events.
- **Is HTTPS Forced** - If you are operating behind a TLS termiantion proxy, set this to `Yes` so that the site can construct proper Web Socket URLs.
- **From Email Name** - The name that system-generated emails appear to originate from.
- **From Email Address** - The email address that system-generated emails originate from.
- **Outgoing Mail Host** - A server which will accept emails from the server running the GRA.
- **Outgoing Mail Port** - Which port to use for submitting emails. Typically 587 or 25.
- **Outgoing Mail Login** - If your mail server requires authentication, the username goes here.
- **Outgoing Mail Password** - If your mail server requires authentication, the password goes here.

You can use the **Send test email to:** facility to verify your email configuration is set correctly.

Ensure you click **Save** after making changes on this page!

## Schedule

The GRA can operate in two different modes:

- If no dates are set on this screen the program is always in an open state.
- When dates are set, the site behaves differently depending on which stage it is in.

The stages are:

1. Prior to registration opening: the site displays a splash page and optionally collects email addresses. Only those with Mission Control access can authenticate.
2. After the **Registration Opens** date: participants can register and browse but not log activities.
3. After the **Program Starts** date: participants can perform activities. If no dates are specified the site is perpetually in this mode.
4. After the **Program Ends** date: participants can still authenticate and see their achievements and earnings but cannot log activities.
5. After the **Access Closed** date: the site displays a splash page and optionally collects email addresses. Only those with Mission Control access can authenticate.

Ensure you click **Save** after making changes on this page!

## Social Media

This page can be used to provide [Open Graph](https://developers.facebook.com/docs/sharing/webmasters/#markup) and [Twitter Card](https://developer.twitter.com/en/docs/tweets/optimize-with-cards/guides/getting-started.html) markup on the front page of the Web site as well as the avatar sharing page.

Ensure you click **Save** after making changes on this page!

## Settings

This page allows modifying settings about how the site operates during programs. Hover over the question mark icon for a more detailed description of what each setting means.

Ensure you click **Save** after making changes on this page!

# Google Analytics Dimensions

The software is configured by default to push two additional dimensions back into Google Analytics. While you can perform reports in the software to view this information, Google Analytics will allow you to discern more information about the source of those users (i.e. what links/sites lead them to your program).

1. The first dimension is named "Program Name" and will push the program name into analytics.
2. The second dimension is named "First Time Participant" and will push that information into analytics if the **Ask if first time** setting is turned on (see the _Settings_ section above).

To be able to report on these dimensions please examine the Google support document [Create and edit custom dimensions and metrics](https://support.google.com/analytics/answer/2709829) to add these two items to the "Custom Dimensions" area. They should be added in the order presented above with a scope of "User" selected.
