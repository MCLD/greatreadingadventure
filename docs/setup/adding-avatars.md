# Adding avatars

Artwork from the game [Glitch](https://www.glitchthegame.com/) was made freely available under a [Creative Commons CC0 1.0 Universal License](http://creativecommons.org/publicdomain/zero/1.0/legalcode) license (essentially a "no rights reserved" license). The artwork used to create in-game characters has been adapted to work with The Great Reading Adventure as the participant's avatar.

The following outline how to download the artwork and incorporate it into your Great Reading Adventure installation to use as avatars.

1. Obtain the `defaultavatars.zip` file from the [GRA release page](https://github.com/MCLD/greatreadingadventure/releases).
2. Unzip the avatars and place them into a directory called `assets` in your main deployment directory (the `assets` directory will be alongside files like `appsettings.json` and `GRA.dll`). Inside the `assets` directory will be the `defaultavatars` directory which will contain all the element directories and installation `.json` files (folders like `Body` and `Hat`, files like `default avatars.json` and `default bundles.json`).
3. Optional: when participants share their avatars via Open Graph markup or Twitter Cards their avatar is placed upon a stage to make it better fit into the image sizes necessary for Open Graph and Twitter: the file named `background.png` will be used as that stage. There are other options in the `backgrounds` folder - if you would like to use one of those just rename the one you select `background.png` and overwrite the current `background.png` file.
4. Access Mission Control on your installation.
5. Choose the picture frame icon from the navigation menu and select "Avatars".
6. Click the "Add default avatars" button.
7. Wait. This takes a while - it may take so long that the Web page itself times out. If you want to see the progress, look in the `shared/logs` directory on your server, it should output as it's importing the various items. You'll see "Default avatars added in *x* seconds." in the log file when it is done.
8. Navigate through the avatar section of the software and ensure that all of the available assets are agreeable for your program(s) and audience.
