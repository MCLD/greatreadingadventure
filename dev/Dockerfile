# Get build image
FROM microsoft/dotnet:2.2-sdk AS dotnet-sdk
WORKDIR /app

# Copy source
COPY . ./

# Add database migrations
WORKDIR /app/src/GRA.Data.SqlServer
RUN dotnet ef migrations add develop

WORKDIR /app/src/GRA.Data.SQLite
RUN dotnet ef migrations add develop

# Publish
WORKDIR /app
RUN dotnet publish -c Release -o "/app/publish/"

# Get runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
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
