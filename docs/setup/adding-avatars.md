# Adding avatars

Artwork from the game [Glitch](https://www.glitchthegame.com/) was made freely available under a [Creative Commons CC0 1.0 Universal License](http://creativecommons.org/publicdomain/zero/1.0/legalcode) license (essentially a "no rights reserved" license). The artwork used to create in-game characters has been adapted to work with The Great Reading Adventure as the participant's avatar.

The following steps outline how to download the artwork and incorporate it into your Great Reading Adventure installation to use as avatars.

## The v4.0.0-noavatars release

If you downloaded the v4.0.0-noavatars release you must download the avatars separately and put them in the correct location to be imported. If you downloaded the larger v4.0.0 release then the avatars are already present in the assets directory.

1. Obtain the `defaultavatars-4.0.0.zip` file from the [GRA release page](https://github.com/MCLD/greatreadingadventure/releases).
2. Unzip the avatars and place them into a directory called `assets` in your main deployment directory (the `assets` directory will be alongside files like `appsettings.json` and `GRA.dll`). Inside the `assets` directory will be the `defaultavatars` directory which will contain all the element directories and installation `.json` files (folders like `Body` and `Hat`, files like `default avatars.json` and `default bundles.json`).

## Loading the avatars

You must load the avatars into the software before they can be used.

1. Optional: when participants share their avatars via Open Graph markup or Twitter Cards their avatar is placed upon a stage to make it better fit into the image sizes necessary for Open Graph and Twitter: the file named `background.png` in the `assets` directory will be loaded into the GRA as that stage. There are other options in the `backgrounds` folder - if you would like to use one of those just rename the one you select `background.png` and overwrite the current `background.png` file.
2. Access Mission Control on your installation.
3. Choose the picture frame icon from the navigation menu and select "Avatars".
4. Click the "Add default avatars" button.
5. Wait. This takes a while - it may take so long that the Web page itself times out. If you want to see the progress, look in the `shared/logs` directory on your server, it should output as it's importing the various items. You'll see "Default avatars added in *x* seconds." in the log file when it is done.
6. Navigate through the avatar section of the software and ensure that all of the available assets are agreeable for your program(s) and audience.
7. The `assets` directory can be removed (if desired) once avatars are loaded.
