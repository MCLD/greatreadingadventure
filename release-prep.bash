#!/usr/bin/env bash

set -o errexit
set -o pipefail

echo "=== Downloading and decompressing avatar package"
curl -L -o defaultavatars.zip https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0/defaultavatars-4.0.0.zip
mkdir -p src/GRA.Web/assets
unzip -q defaultavatars.zip -d src/GRA.Web/assets
rm defaultavatars.zip
