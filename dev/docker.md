# GRA Developer Documentation - Docker

The GRA is designed to function in Docker utilizing [Microsoft-provided base images](https://hub.docker.com/r/microsoft/). If built using `Dockerfile`s in the project, the [aspnetcore-build](https://hub.docker.com/r/microsoft/aspnetcore-build/) image will be used for the build and the [aspnetcore](https://hub.docker.com/r/microsoft/aspnetcore/) image will be used for running the compiled application. The project contains two `Dockerfile`s:

- `Dockerfile` - builds the project as-is, used for images tagged as release or `master`.
- `dev/Dockerfile` - adds database migrations and builds the project, used for the image tagged as `develop`.

## Sample build command

```sh
docker build . -t gra:latest
```

## Sample run command(s)

### Simple
```sh
docker run -d -p 80:80 --restart unless-stopped -v/mnt/sharedstorage:/app/shared --name gra mcld/gra:latest
```

### More complex
```sh
docker run -d -p 80:80 \
  --name gra-instance1 \
  --restart unless-stopped \
  -e GraInstanceName=gra-instance1 \
  -e "GraDeployDate=`date +'%x %H:%M:%S'`" \
  -v /mnt/sharedstorage:/app/shared \
  -v /mnt/avatarassets:/app/assets \
  mcld/gra:latest
```
