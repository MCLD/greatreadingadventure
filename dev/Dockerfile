# Get build image
FROM microsoft/dotnet:1.1-sdk AS dotnet-sdk
WORKDIR /app

# Copy source and build
COPY . ./
RUN dotnet restore

# Add database migrations
WORKDIR /app/src/GRA.Data.SqlServer
RUN dotnet ef migrations add -s ../GRA.Web/GRA.Web.csproj develop

WORKDIR /app/src/GRA.Data.SQLite
RUN dotnet ef migrations add -s ../GRA.Web/GRA.Web.csproj develop

# Build and publish
WORKDIR /app
RUN dotnet publish -c Release -o "$(pwd)/publish/web"

# Get runtime image
FROM microsoft/dotnet:1.1-runtime
WORKDIR /app

# Bring in metadata
ARG commit=unknown

LABEL gra.commit=$commit
LABEL maintainer="Maricopa County Library District developers <development@mcldaz.org>"

# Copy source
COPY --from=dotnet-sdk /app/publish/web .

# Persist shared directory
VOLUME ["/app/shared"]

# Port 80 for http
EXPOSE 80

# Set entrypoint
ENTRYPOINT ["dotnet", "GRA.Web.dll"]
