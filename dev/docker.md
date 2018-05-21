# GRA Developer Documentation - Docker

The GRA is designed to function in Docker utilizing [Microsoft-provided base images](https://hub.docker.com/r/microsoft/). If built using a `Dockerfile` in the project, the [aspnetcore-build](https://hub.docker.com/r/microsoft/aspnetcore-build/) image will be used for the build and the [aspnetcore](https://hub.docker.com/r/microsoft/aspnetcore/) image will be used for running the compiled application. You must choose a single `Dockerfile` to use for your build:

- `Dockerfile` - builds the project as-is, used for images tagged as release or `master`.
- `dev/Dockerfile` - adds database migrations and builds the project, used for the image tagged as `develop`.

## Sample build command

### Dockerfile for releases
```sh
docker build . -t gra:latest
```

### Dockerfile for development
```sh
docker build -f dev/Dockerfile . -t gra:develop
```

## Sample run command(s)

### Run a locally-built image
```sh
docker run -d -p 80:80 \
  --name gra
  --restart unless-stopped \
  -v /mnt/sharedstorage:/app/shared \
  -v /mnt/avatarassets:/app/assets \
  gra:latest
```

### Download and run an image from Docker Hub (simple)
```sh
docker run -d -p 80:80 \
  --name gra \
  --restart unless-stopped \
  -v /mnt/sharedstorage:/app/shared \
  -v /mnt/avatarassets:/app/assets \
  mcld/gra:latest
```

### Download and run an image from Docker Hub (complex)

This example is for a multi-instance environment and includes additional environment settings (including passing in database connection information via the environment rather than a configuration file).

```sh
docker run -d -p 80:80 \
  --name gra-instance1 \
  --restart unless-stopped \
  -e "ConnectionStrings:SqlServer=Server=dbserver;Database=gra;user id=grauser;password=supersecret;MultipleActiveResultSets=true"
  -e TZ=US/Arizona \
  -e GraInstanceName=gra-instance1 \
  -e "GraDeployDate=`date +'%x %H:%M:%S'`" \
  -v /mnt/sharedstorage:/app/shared \
  -v /mnt/avatarassets:/app/assets \
  mcld/gra:latest
```
