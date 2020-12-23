# Customizing the look and feel

The participant-facing pages can be fully customized to support your specific needs.

## Custom landing pages and dashboard

The GRA has two operating modes, either without schedule or with scheduled dates.

1. The default operation of the GRA means that as soon as you install the software the program is open and running for registration and for participants to log activities. In this instance the software considers the program to always be in the "open" state and uses that template for the landing page.

2. Through "Site management" in Mission Control a schedule can be configured dividing the program up into the following stages:
   1. Before registration opens
   2. Registration open
   3. Program starts
   4. Program ends (but accounts are still accessible)
   5. Access closed

### Templates

When the GRA starts up, it will create a `templates/Home` directory in the `shared` directory containing template files which are used for displaying the landing page(s) and dashboard. If you want to customize these pages you can copy the files from `templates/Home` into `views/Home` and then modify them.

- `IndexBeforeRegistration.cshtml` is shown during stage 1 above, before registration has opened.
- `IndexRegistrationOpen.cshtml` is shown during stage 2 above, once registration is opened but before participants can log activity.
- `IndexProgramOpen.cshtml` is shown during stage 3 when the program is open for participants.
- `IndexProgramEnded.cshtml` is shown during stage 4 when the program has closed, however participants may need to access the site to retreive prize codes or review their mail.
- `IndexAccessClosed.cshtml` is shown during stage 5 when participants may no longer sign in.
- `Dashboard.cshtml` is shown when a participant logs in.

## Global styles and scripts

Starting with version 4.1.1, custom styles and scripts can be configured which will be added to every page site-wide. Note that the GRA will only check if these files have been changed every 60 minutes by default. The site setting "Check for site.css and site.js changes on disk" under Web in Site management can be changed to a number (in minutes) that you'd like to check for these files to have changed. Set the value to 0 while editing the files to see them refresh every time you hit reload.

### Styles

Additional CSS styling can be added in the `shared` directory: create a subdirectory called `styles` and place a `site.css` file in it. This CSS file is loaded last so any changes provided in it should take precedence over built-in CSS styles. As an example: if you'd like to make the background of the navigation bar light blue, place the following in `shared/styles/site.css`:

```css
.gra-navbar {
  background-color: Azure;
}
```

### Scripts

Custom JavaScript that you'd like injected into the site can be added in the file: `shared/scripts/site.js`. This file is loaded after all of the other JavaScript so elements should be available for you to access or modify.

## Home screen icons and favicon.ico

When accessing your site, participants will see the Great Reading Adventure logo in their URL bar (often referred to as a 'favicon'). They'll also see the GRA logo if they add your site to their home screen. If you'd like to customize this, you can replace the icons and images in `shared/content/` with ones of your choosing. Performing an Internet search for terms like "favicon generator" will lead you to tools which will help you resize your image or logo to the appropriate size(s) to replace the images in `shared/content/`.
