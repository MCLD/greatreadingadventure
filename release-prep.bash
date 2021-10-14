#!/usr/bin/env bash

set -o errexit
set -o pipefail

echo "=== Downloading and decompressing avatar package"
curl -Lo defaultavatars.tgz https://github.com/MCLD/gra-avatars/archive/v4.2.1.tar.gz
mkdir -p src/GRA.Web/assets/defaultavatars
tar -xzf defaultavatars.tgz --strip-components=1 -C src/GRA.Web/assets/defaultavatars
rm defaultavatars.tgz
