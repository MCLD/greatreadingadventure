# GRA Developer Documentation - Docker

The GRA is designed to function in Docker utilizing [Microsoft-provided base images](https://azure.microsoft.com/en-us/product-categories/containers/). If built using a `Dockerfile` in the project, [Microsoft's docker images](https://github.com/dotnet/dotnet-docker) will be used for the basis of build and publish.

## Sample build command

```sh
bash ./docker-build.bash
```

## Sample run command(s)

### Run a locally-built image
```sh
docker run -d -p 80:80 \
  --name gra
  --restart unless-stopped \
  -v /mnt/sharedstorage:/app/shared \
  gra:latest
```

### Download and run an image from Docker Hub (simple)
```sh
docker run -d -p 80:80 \
  --name gra \
  --restart unless-stopped \
  -v /mnt/sharedstorage:/app/shared \
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
  mcld/gra:latest
```
