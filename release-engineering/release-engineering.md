## Code changes for a new version

In order to ensure the new version is reflected throughout the code, the following files must be
modified:

1. The `AssemblyVersion` and `AssemblyFileVersion` attributes in:
  1. `GRA.Communications/Properties/AssemblyInfo.cs`
  2. `GRA.Database/Properties/AssemblyInfo.cs`
  3. `GRA.Logic/Properties/AssemblyInfo.cs`
  4. `GRA.Tools/Properties/AssemblyInfo.cs`
  5. `SRP/Properties/AssemblyInfo.cs`
  6. `SRP_DAL/Properties/AssemblyInfo.cs`
  7. `SRPUtilities/Properties/AssemblyInfo.cs`
  8. The last insert in the `SRP/ControlRoom/Modules/Install/InsertInitialData*.sql` files
2. The manual version should be updated in `docs/conf.py`

## Additional files

The following files in the root of the SRP directory (not the root of the project!) will be deployed and should be verified:

1. `README.md`
2. `LICENSE`

## Producing the release

1. Clone the latest version of the code from GitHub to a new directory.
2. Open the solution file in Visual Studio.
3. Right-click on the solution and select *Build Solution*. NuGet should automatically restore packages.
4. Right-click on the SRP project and select *Publish*.
5. Ensure the setting *Precompile during publishing* is selected.
6. Select the *Configure* option next to *Precompile during publishing* and select the *Allow precompiled site to be updatable* option.
7. Create a zip file, name it `GreatReadingAdventure-#.#.#.zip`.
8. Create a [new release](https://github.com/MCLD/greatreadingadventure/releases) and attach release notes and the zip file.
9. Update the [README](https://github.com/MCLD/greatreadingadventure/edit/master/README.md) on the GitHub repository with the link to the new release.
10. Update the *Download the GRA* link on the [Web site](https://github.com/MCLD/greatreadingadventure/edit/gh-pages/index.html) as well as the structured data at the top of the file (e.g. `downloadUrl` and `version`)
11. Update the [forum](http://forum.greatreadingadventure.com/) to reflect the latest release.
