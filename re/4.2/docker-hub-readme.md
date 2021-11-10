# Quick Reference

- **Maintained by**: [Maricopa County Library District software developers](https://github.com/MCLD)

- **Where to get help**: [Great Reading Adventure manual](http://manual.greatreadingadventure.com/), [GitHub Discussions](https://github.com/MCLD/greatreadingadventure/discussions)

# Supported tags and respective `Dockerfile` links

- [`v4.2.1`, `latest`](https://github.com/MCLD/greatreadingadventure/blob/v4.2.1/Dockerfile)
- [`develop`](https://github.com/MCLD/greatreadingadventure/blob/develop/Dockerfile)

# What is the Great Reading Adventure

<img src="https://raw.githubusercontent.com/mcld/greatreadingadventure/develop/src/GRA.Web/wwwroot/images/great-reading-adventure-logo%401x.png"
     alt="Great Reading Adventure logo"
     align="right">

The Great Reading Adventure is a robust, open source software designed to manage library reading programs online. The GRA is free to use, modify, and share. Check out [www.greatreadingadventure.com](https://www.greatreadingadventure.com/) for an overview of its functionality and capabilities and [the manual](http://manual.greatreadingadventure.com/) for information about installing and using it.

# How to use this image

```
docker run -d -p 80:80 \
    --name gra \
    --restart unless-stopped \
    -e "ConnectionStrings:SqlServer=Server=dbserver;Database=gra;user id=grauser;password=supersecret;MultipleActiveResultSets=true" \
    -v /gra/shared:/app/shared \
    mcld/gra
```

Assuming you have a valid SQL Server connection string and that port 80 isn't in use on the host machine, you can now navigate to `http://localhost/`. If port 80 is in use, you can use `-p 8000:80` and navigate to `http://localhost:8000/` to access the site.

**Note that you will probably want to map a local directory to `/app/shared` in the container so that uploaded files are kept if the container is destroyed and recreated, so that you can edit site files, and so that you can add files to the site as needed.**

# Image Variants

The `gra` images come in several variants for different audiences. They are based on the Microsoft [`aspnet`](https://hub.docker.com/_/microsoft-dotnet-aspnet) image.

## `gra:latest`

This is an image containing the latest released version of The Great Reading Adventure along with associated default avatars, it is probably what you want.

## `gra:<version>`

This is an image containing a specific version of The Great Reading Adventure along with associated default avatars.

## `gra:develop`

This image contains the latest development build of The Great Reading Adventure. It does not contain the default package of avatars and, while it should be stable, it may have new and experimental code in it.

# License

The Great Reading Adventure source code is distributed under [The MIT License](https://opensource.org/licenses/MIT). For other included packages, please see the [credits](https://github.com/MCLD/greatreadingadventure/blob/develop/CREDITS.md) file.

The Great Reading Adventure was initially developed by the [Maricopa County Library District](https://www.mcldaz.org/) with support by the [Arizona State Library, Archives and Public Records](https://www.azlibrary.gov/), a division of the Secretary of State, with federal funds from the [Institute of Museum and Library Services](https://www.imls.gov/).
