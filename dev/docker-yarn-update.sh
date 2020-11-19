#!/bin/sh

# How to run this:
# 1. Find a Linux system (Windows Docker path sharing is a mess)
# 2. Clone the repository
# 4. Run the following: docker run -it --rm -v `pwd`:/app mcr.microsoft.com/dotnet/core/sdk:3.0 bash /app/dev/docker-yarn-update.sh
# 5. See what happened with git status

curl -sL https://deb.nodesource.com/setup_12.x | bash -
curl -sL https://dl.yarnpkg.com/debian/pubkey.gpg | apt-key add -
echo "deb https://dl.yarnpkg.com/debian/ stable main" | tee /etc/apt/sources.list.d/yarn.list
apt-get update && apt-get -y install yarn

rm -rf /app/src/GRA.Web/node_modules/*
cd /app/src/GRA.Web && yarn install --check-files
# FontAwesome
rm -rf /app/src/GRA.Web/wwwroot/webfonts/*
mkdir -p /app/src/GRA.Web/wwwroot/webfonts
cp /app/src/GRA.Web/node_modules/@fortawesome/fontawesome-free/webfonts/* /app/src/GRA.Web/wwwroot/webfonts/
# Slick
rm -rf /app/src/GRA.Web/wwwroot/css/fonts/*
mkdir -p /app/src/GRA.Web/wwwroot/css/fonts
cp /app/src/GRA.Web/node_modules/slick-carousel/slick/fonts/* /app/src/GRA.Web/wwwroot/css/fonts/
rm /app/src/GRA.Web/wwwroot/css/ajax-loader.gif
cp /app/src/GRA.Web/node_modules/slick-carousel/slick/ajax-loader.gif /app/src/GRA.Web/wwwroot/css/

# Build to activate bundler/minifier
cd /app/src/GRA.Web && dotnet restore && dotnet bundle
