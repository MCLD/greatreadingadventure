#!/usr/bin/env bash

set -o errexit
set -o pipefail

readonly BLD_PUBLISH_DIRECTORY=GreatReadingAdventure-${BLD_RELEASE_VERSION}

echo "=== Installing necessary packages"
apt update && apt install build-essential zip -y

cd /

echo "=== Making directory $BLD_PUBLISH_DIRECTORY"
mkdir -p "$BLD_PUBLISH_DIRECTORY"

echo "=== Copying files into $BLD_PUBLISH_DIRECTORY"
cp -a app/* "$BLD_PUBLISH_DIRECTORY"

echo "=== Compressing files"
zip -q -r9 "GreatReadingAdventure-$BLD_RELEASE_VERSION.zip" "$BLD_PUBLISH_DIRECTORY"/

du -sch "$BLD_PUBLISH_DIRECTORY"/ "GreatReadingAdventure-$BLD_RELEASE_VERSION.zip"

if [[ -n $BLD_RELEASE_TOKEN ]]; then
  curl -L -O https://github.com/tfausak/github-release/releases/latest/download/github-release-linux.gz
  gunzip github-release-linux.gz && chmod 700 github-release-linux && \
  ./github-release-linux upload \
    --token "$BLD_RELEASE_TOKEN" \
    --owner "$BLD_RELEASE_OWNER" \
    --repo "$BLD_RELEASE_REPO" \
    --tag "v$BLD_RELEASE_VERSION" \
    --file "GreatReadingAdventure-$BLD_RELEASE_VERSION.zip" \
    --name "GreatReadingAdventure-$BLD_RELEASE_VERSION.zip"
else
  echo "=== No BLD_RELEASE_TOKEN configured, not pushing release artifacts to GitHub Releases"
fi
