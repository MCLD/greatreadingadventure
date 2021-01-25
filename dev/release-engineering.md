# GRA Developer Documentation - Release engineering

## Overview

Currently release packages are built using our GitHub Actions CI process. This process relies on [MCLD/buildscript](https://github.com/MCLD/buildscript) for build and release logic.

## Creating a release

### Baseline tasks

1. Ensure the project builds.
2. Ensure all tests pass.
3. Ensure there are no outdated packages (or if there are, that there's a reason why they aren't being updated).
4. Ensure any new images are optimized (e.g. `ImageOptim`, `pngcrush`, `guetzli`, `svgo`, etc.).

### Create the branch

Create a branch of the project named `release/x.y.z` (include `z` even if it is 0). **Do not push this into the main GRA repository**. Only push this to your fork on GitHub. If you are worried you will forget this step, perform these actions in a separate local folder that doesn't have the `MCLD/greatreadingadventure` remote configured (i.e. clone into another folder based off your fork, make all these changes, and then at the final step add the `MCLD/greatreadingadventure` remote and push to it).

### Update documentation

Areas to check:

- `README.md` - GitHub front matter
- `CREDITS.md` - For added/removed items
- `dev/` - Developer documentation
- `docs/` - Software manual
- Search all files in project for the prior GRA version and update
- If there's been a framework upgrade, search all files in the project for the old framework version

### Check avatar project is up to date

If there have been changes to the avatars, take these steps:

1. Ensure the [MCLD/gra-avatars](https://github.com/MCLD/gra-avatars) project is up-to-date and has a successfully built release .ZIP file. There's no build process here - GitHub will produce a .ZIP file of the repository when you create a release.
2. Test the .ZIP file produced by GitHub (import it locally by placing it in `shared/private/` named `defaultavatars.zip`. The avatar file will not upload by default through the Web interface due to its size).
3. In the GRA project, update `release-prep.bash` to reference the new avatar release file.

When the project's GitHub Action runs against a git branch named in the format `release/x.y.z`, it will download and incorporate the avatars into the Docker image and final package.

### Update project versions

1. Update the `<Version>` tag in each `.csproj` file in the project.
2. Also update the `<FileVersion>` tag in the `GRA.Web.csproj` file.

### Changelog

1. Add a header under `Unreleased` with the version number and release date.
2. Add a link at the bottom of the file to go to the expected GitHub release URL for that release.

### Release notes

1. Create a release notes file in `re/x.y/Release Notes x.y.z.md`. Use existing release notes as a template.
2. Update or create the `re/x.y/docker-hub-readme.md` file.

### Test building the release

1. Enable Actions for your fork of the repository (`https://github.com/<you>/greatreadingadventure/settings/actions/`).
2. To test uploads to the [GitHub Container Registry](https://docs.github.com/en/free-pro-team@latest/packages/guides/about-github-container-registry):
   1. Generate a [Personal Access Token](https://docs.github.com/en/free-pro-team@latest/github/authenticating-to-github/creating-a-personal-access-token) with [package privileges](https://docs.github.com/en/free-pro-team@latest/packages/guides/about-github-container-registry#about-scopes-and-permissions-for-github-container-registry).
   2. Add a [repository secret](https://docs.github.com/en/free-pro-team@latest/actions/reference/encrypted-secrets#creating-encrypted-secrets-for-a-repository) called `GHCR_USER` containing your GitHub username (for both authentication and the repository to upload to).
   3. Add a repository secret called `GHCR_PAT` containing your PAT generated above.
3. To test uploads to Docker Hub:
   1. Generate a [Personal Access Token](https://docs.docker.com/docker-hub/access-tokens/).
   2. Add a repository secret called `CR_USER` containing your Docker ID (for authentication).
   3. Add a repository secret called `CR_OWNER` containing your Docker ID (for the repository to upload to).
   4. Add a repository secret called `CR_PASSWORD` containing your PAT generated above.
4. Push your local `release/x.y.z` branch to **your fork** of the `greatreadingadventure` project on GitHub.
5. Monitor the Actions tab to see if the build is successful.

If everything worked, you should see:

- An uploaded Docker image to the GitHub Container Registry (if configured).
- An uploaded Docker image to Docker Hub (if configured).
- A .ZIP file attached to the "Build - action" workflow.

### Test the release

1. Test the Docker image in a Linux Docker container environment and ensure it works properly.
2. Test the .ZIP attached to the "Build - action" workflow (ideally in a Windows environment) and ensure it works properly.

### Execute the release

1. Push your `release/x.y.z` branch to the [MCLD/greatreadingadventure](https://github.com/MCLD/greatreadingadventure) repository.
2. Once the build is complete, verify Docker images were pushed to the [GitHub Container Registry](https://github.com/orgs/MCLD/packages/container/package/gra) and [Docker Hub](https://hub.docker.com/r/mcld/gra/tags?page=1&ordering=last_updated) correctly.
3. Download the .ZIP file attached to the "Build - action" workflow. Unzip that, inside you should find a single file: `GreatReadingAdventure-vx.y.z.zip`.
4. Open a pull request to merge the `release/x.y.z` branch into `develop`.
5. Perform the merge once the actions all clear.
6. [Draft a new release](https://github.com/MCLD/greatreadingadventure/releases/new) on GitHub.
7. In the "Tag version" field, enter `vx.y.z` and ensure it's targeted to `develop`.
8. In the "Release title" field, enter "Great Reading Adventure vx.y.z".
9. Paste the release notes into the "Describe this release" field.
10. Upload the `GreatReadingAdventure-vx.y.z.zip` to your release. Ensure it uploads successfully.
11. Publish the release and post to the discussions!
