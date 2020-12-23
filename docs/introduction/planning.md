# Planning for initial configuration

## Hosting

As a Web application, the first decision that will need to be made is about hosting. Hosting requirements can be found in the [system requirements](../../installation/system-requirements) section of this manual.

### Self-hosting

If your organization has the proper Linux, macOS, or Windows server environment that is accessible from the Internet, hosting on your own systems can be an option. You can host the application on an existing Web server or host the application in a container using Docker.

### Paid hosting

Various providers are available which can host the GRA for a monthly fee.

## Application configuration

### Programs

Setting up multiple programs gives the flexibility to deliver targeted content to varying audiences keeping key elements unified for reporting and statistics. You have the freedom to customize the content of each program so that participants can access audience-appropriate activities, badges, and events. Users can self-select which program to enroll in or the system can automatically place them into an age-appropriate program, based on age or school grade.

Setting up multiple programs adds flexibility but increases complexity.

The default installation of the GRA sets up four programs defined by age:

- Prereaders (ages 4 and below)
- Kids (ages 5 to 11)
- Teens (ages 12 to 17)
- Adults (ages 18 and up)

These default programs are configured for participants to enter a number of minutes that they read every day with each minute equating to one point in the program. The translation of activity (minutes read) to points earned can be modified once installed, however at this time there is one translation for all programs.

If you would prefer the initial setup to be a single program which is configured to earn a single point for each book read you can set the `GraInitialProgramSetup` configuration value to "single".

Creating a **single reading program** is good if you:

- Are targeting a single age group
- Intend to have the same experience for all participants
- Aren't intending to require a literacy test for only some participants

**Several age-specific reading programs** are better if you:

- Want to report on age groups differently
- Intend to have Badges, Challenges, and Events specific to age groups
