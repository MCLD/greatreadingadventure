# Get build image
FROM microsoft/dotnet:2.1-sdk AS dotnet-sdk
WORKDIR /app

# Copy source
COPY . ./

# Run restore and build
RUN dotnet build -c Release

# Publish
RUN dotnet publish -c Release -o "/app/publish/" --no-build

# Get runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app

# Bring in metadata
ARG commit=unknown
ARG branch=unknown

LABEL commit=$commit
LABEL branch=$branch
LABEL maintainer="Maricopa County Library District developers <development@mcldaz.org>"

# Copy source
COPY --from=dotnet-sdk "/app/publish/" .

# Port 80 for http
EXPOSE 80

# Set entrypoint
ENTRYPOINT ["dotnet", "GRA.Web.dll"]
