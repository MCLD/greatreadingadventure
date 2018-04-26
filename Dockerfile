# Get build image
FROM microsoft/aspnetcore-build:1.1 AS build-env
WORKDIR /app

# Copy source and build
COPY . ./
RUN dotnet restore

# Build and publish
RUN dotnet publish -c Release -o "$(pwd)/publish/web"

# Get runtime image
FROM microsoft/aspnetcore:1.1
WORKDIR /app

LABEL maintainer="Maricopa County Library District developers <development@mcldaz.org>"

# Copy source
COPY --from=build-env /app/publish/web .

# Persist shared directory
VOLUME ["/app/shared"]

# Port 80 for http
EXPOSE 80

# Set entrypoint
ENTRYPOINT ["dotnet", "GRA.Web.dll"]
