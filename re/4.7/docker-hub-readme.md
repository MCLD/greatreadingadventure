# Quick Reference

- **Maintained by**: [Maricopa County Library District software developers](https://github.com/MCLD)

- **Where to get help**: [Great Reading Adventure manual](http://manual.greatreadingadventure.com/), [GitHub Discussions](https://github.com/MCLD/greatreadingadventure/discussions)

# Supported tags and respective `Dockerfile` links

- [`v4.7.0`](https://github.com/MCLD/greatreadingadventure/blob/v4.7.0/Dockerfile) - latest stable release
- [`latest`](https://github.com/MCLD/greatreadingadventure/blob/main/Dockerfile) - current development tree

# What is the Great Reading Adventure?

<img src="https://raw.githubusercontent.com/mcld/greatreadingadventure/main/src/GRA.Web/wwwroot/images/great-reading-adventure-logo%401x.png"
     alt="Great Reading Adventure logo"
     align="right">

The Great Reading Adventure is a robust, open source software designed to manage library reading programs online. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](https://www.greatreadingadventure.com/) for an overview of its functionality and capabilities and [the manual](http://manual.greatreadingadventure.com/) for information about installing and using it.

Source code to The Great Reading Adventure can be found on [GitHub](https://github.com/MCLD/greatreadingadventure) and [Codeberg](codeberg.org/MCLD/greatreadingadventure).

# How to use this image

## Evaluating the software without saving any configuration/data/settings

If you just want to try the GRA out you can use SQLite as a database backend and not map a directory to save uploaded files. **This is not recommended for a production environment - when you stop this Docker container you will lose all data and configuration**.

```
docker run -d -p 1234:8080 \
    --name gra \
    --restart unless-stopped \
    -e "GraConnectionStringName=SQLite" \
    mcld/gra
```

Access `http://localhost:1234/` in a browser on the local machine to access the GRA.

## Running with persistent files and a SQL Server database

Here's an example command to run with a SQL Server connection string for the backend database connection and a mapped directory for uploaded files.

```
docker run -d -p 80:8080 \
    --name gra \
    --restart unless-stopped \
    -e "ConnectionStrings:SqlServer=Server=dbserver;Database=gra;user id=grauser;password=supersecret;MultipleActiveResultSets=true;Encrypt=false" \
    -v /gra/shared:/app/shared \
    mcld/gra
```

Assuming you have a valid SQL Server connection string and that port 80 isn't in use on the host machine, you can now navigate to `http://localhost/`. If port 80 is in use, you can update the port mapping in the command to `-p 8080:8080` to and navigate to `http://localhost:8080/` to access the site.

**Note that you will probably want to map a local directory to `/app/shared` in the container so that uploaded files are kept if the container is destroyed and recreated, so that you can edit site files, and so that you can add files to the site as needed.**

More information about running the GRA can be found in the manual: [Install and run the GRA in Docker](https://manual.greatreadingadventure.com/en/latest/installation/install-the-software/#install-and-run-the-gra-in-docker).

# Image Variants

The `gra` images come in several variants for different audiences. They are based on the Microsoft [`aspnet`](https://hub.docker.com/_/microsoft-dotnet-aspnet) image.

## `gra:v4.7.0`

This is an image containing the latest released version of The Great Reading Adventure along with associated default avatars: if you are an end user, it is probably what you want.

## `gra:latest`

This image contains the latest development build of The Great Reading Adventure. It does not contain the default package of avatars and, while it should be stable, it may have new and experimental code in it.

If you want to use avatars with this build you will need to [download them](https://github.com/MCLD/gra-avatars/releases) and then [upload into the software](https://manual.greatreadingadventure.com/en/latest/setup/adding-avatars/) yourself.

# License

The Great Reading Adventure source code is distributed under [The MIT License](https://opensource.org/licenses/MIT). For other included packages, please see the [credits](https://github.com/MCLD/greatreadingadventure/blob/main/CREDITS.md) file.

The Great Reading Adventure was initially developed by the [Maricopa County Library District](https://www.mcldaz.org/) with support by the [Arizona State Library, Archives and Public Records](https://www.azlibrary.gov/), a division of the Secretary of State, with federal funds from the [Institute of Museum and Library Services](https://www.imls.gov/).

