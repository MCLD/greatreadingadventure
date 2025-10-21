#!/bin/sh

# How to run this:
# 1. Find a Linux system (Windows Docker path sharing is a mess)
# 2. Clone the repository
# 4. Run the following: docker run -it --rm -v `pwd`:/app node:latest bash /app/dev/docker-yarn-update.sh
# 5. See what happened with git status

cd /app/src/GRA.Web && yarn install

mkdir -p /app/src/GRA.Web/js
rm -rf /app/src/GRA.Web/js/*.js

cp /app/src/GRA.Web/node_modules/@eonasdan/tempus-dominus/dist/js/tempus-dominus.min.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/@popperjs/core/dist/umd/popper.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/bootstrap/dist/js/bootstrap.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/commonmark/dist/commonmark.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/jquery/dist/jquery.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/jquery-validation/dist/jquery.validate.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/jquery-validation/dist/localization/messages_es.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/moment/min/moment.min.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/moment-timezone/builds/moment-timezone.min.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/qr-code-styling/lib/qr-code-styling.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/slick-carousel/slick/slick.js /app/src/GRA.Web/js
cp /app/src/GRA.Web/node_modules/tom-select/dist/js/tom-select.complete.js /app/src/GRA.Web/js


mkdir -p /app/src/GRA.Web/css
rm -rf /app/src/GRA.Web/css/*.css

cp /app/src/GRA.Web/node_modules/@eonasdan/tempus-dominus/dist/css/tempus-dominus.css /app/src/GRA.Web/css
cp /app/src/GRA.Web/node_modules/@fortawesome/fontawesome-free/css/all.css /app/src/GRA.Web/css
cp /app/src/GRA.Web/node_modules/@fortawesome/fontawesome-free/css/v4-shims.css /app/src/GRA.Web/css
cp /app/src/GRA.Web/node_modules/bootstrap/dist/css/bootstrap.css /app/src/GRA.Web/css
cp /app/src/GRA.Web/node_modules/slick-carousel/slick/slick.css /app/src/GRA.Web/css
cp /app/src/GRA.Web/node_modules/slick-carousel/slick/slick-theme.css /app/src/GRA.Web/css
cp /app/src/GRA.Web/node_modules/tom-select/dist/css/tom-select.bootstrap5.css /app/src/GRA.Web/css

# FontAwesome
rm -rf /app/src/GRA.Web/wwwroot/webfonts/*
mkdir -p /app/src/GRA.Web/wwwroot/webfonts
cp /app/src/GRA.Web/node_modules/@fortawesome/fontawesome-free/webfonts/* /app/src/GRA.Web/wwwroot/webfonts/

# Slick
rm -rf /app/src/GRA.Web/wwwroot/css/fonts
mkdir -p /app/src/GRA.Web/wwwroot/css/fonts
cp /app/src/GRA.Web/node_modules/slick-carousel/slick/fonts/* /app/src/GRA.Web/wwwroot/css/fonts/
rm /app/src/GRA.Web/wwwroot/css/ajax-loader.gif
cp /app/src/GRA.Web/node_modules/slick-carousel/slick/ajax-loader.gif /app/src/GRA.Web/wwwroot/css/

echo --- GRA.Web yarn outdated && yarn outdated

