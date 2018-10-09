# Get build image
FROM microsoft/aspnetcore-build:1.1 AS dotnet-sdk
WORKDIR /app

# Copy source
COPY . ./

# Restore
RUN dotnet restore

# Publish
RUN dotnet publish -c Release -o "/app/publish/"

# Get runtime image
FROM microsoft/aspnetcore:1.1
WORKDIR /app

# Bring in metadata
ARG commit=unknown
ARG branch=unknown

LABEL commit=$commit
LABEL branch=$branch
LABEL maintainer="Maricopa County Library District developers <development@mcldaz.org>"

# Copy source
COPY --from=dotnet-sdk "/app/publish/" .

# Persist shared directory
VOLUME ["/app/shared"]

# Port 80 for http
EXPOSE 80

# Set entrypoint
ENTRYPOINT ["dotnet", "GRA.Web.dll"]
