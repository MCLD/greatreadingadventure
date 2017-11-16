# Get build image
FROM microsoft/aspnetcore-build:1.1 AS build-env
WORKDIR /app

# Copy source and build
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o $(pwd)/publish/web

# Get runtime image
FROM microsoft/aspnetcore:1.1
WORKDIR /app

# Copy source
COPY --from=build-env /app/publish/web .

# Set entrypoint
ENTRYPOINT ["dotnet", "GRA.Web.dll"]

