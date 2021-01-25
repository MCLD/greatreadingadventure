# Setting up your administrator account

Once you can access your installation of the software, it's time to set up the initial administrator account. To do this, you'll need the initial Authorization Code (configured as `GraInitialAuthCode`) that you put into the `appsettings.json` file.

## Joining with an Authorization Code

The easiest approach is to join and grant yourself administrative rights at the same time.

1. Visit `/Join/AuthorizationCode/` in your installation
2. Supply the Authorization Code specified as `GraInitialAuthCode` in your configuration file
3. Once you've completed the process you'll be able to click the rocket in the navigation bar at the top to access Mission Control

## Joining with a regular account

1. Select the "Join" link or visit `/Join/` in your installation.
2. Complete the process to create your account.
3. Visit `/MissionControl/` in your installation
4. Supply the Authorization Code specified as `GraInitialAuthCode` in your configuration file
5. The system will grant you full rights and place you in Mission Control
