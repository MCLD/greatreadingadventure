#!/usr/bin/env bash

# @(#) build.bash - shell script for building docker images
#
# Copyright (C) 2018 Maricopa County Library District, All Rights Reserved.
# Releaesd under The MIT License
# https://github.com/MCLD/greatreadingadventure/blob/develop/LICENSE
#
# Second revision based on "Minimal safe Bash script template" by Maciej Radzikowski
#   https://betterdev.blog/minimal-safe-bash-script-template/

set -Eeuo pipefail
trap cleanup SIGINT SIGTERM ERR EXIT

usage() {
  cat <<EOF
Usage: $(basename "${BASH_SOURCE[0]}") [-h] [-v] [-df Dockerfile] [-p] [Docker tag]

Build the project in the current directory using Docker.

Available options:

-h, --help         Print this help and exit
-v, --verbose      Print script debug info
-df, --dockerfile  Use the specified Dockerfile
-p, --publish      Run the release-publish.bash script in the container (if it's present)

Environment variables:

- BLD_DOCKER_IMAGE - optional - name of Docker image, uses directory name by default
- CR_HOST - optional - hostname of the container registry, defaults Docker default (Docker Hub)
- CR_OWNER - optional - owner of the container registry
- CR_PASSWORD - optional - password to log into the container registry
- CR_USER - optional - username to log in to the container registry
- DOCKER_LOCK_VERSION - optional - a version of docker-lock to use (e.g. 0.8.10), can also be
                                   specified in docker-lock-version.txt
- GHCR_OWNER - optional - owner of the GitHub Container Registry (defaults to GHCR_USER)
- GHCR_PAT - optional - GitHub Container Registry Personal Access Token
- GHCR_USER - optional - username to log in to the GitHub Container Registry

Version 1.2.0 released 2022-12-13
EOF
  exit
}

cleanup() {
  trap - SIGINT SIGTERM ERR EXIT
  # script cleanup here
}

setup_colors() {
  if [[ -t 2 ]] && [[ -z "${NO_COLOR-}" ]] && [[ "${TERM-}" != "dumb" ]]; then
    NOFORMAT='\033[0m' RED='\033[0;31m' GREEN='\033[0;32m' ORANGE='\033[0;33m' BLUE='\033[0;34m' PURPLE='\033[0;35m' CYAN='\033[0;36m' YELLOW='\033[1;33m'
  else
    NOFORMAT='' RED='' GREEN='' ORANGE='' BLUE='' PURPLE='' CYAN='' YELLOW=''
  fi
}

msg() {
  echo >&2 -e "${1-}"
}

die() {
  local msg=$1
  local code=${2-1} # default exit status 1
  msg "$msg"
  exit "$code"
}

parse_params() {
  dockerfile=''
  publish=0
  
  while :; do
    case "${1-}" in
      -h | --help) usage ;;
      -v | --verbose) set -x ;;
      --no-color) NO_COLOR=1 ;;
      -df | --dockerfile)
        dockerfile="${2-}"
        shift
      ;;
      -p | --publish)
        publish=1
      ;;
      -?*) die "Unknown option: $1" ;;
      *) break ;;
    esac
    shift
  done
  
  readonly dockertag="${1-}"
  
  return 0
}

parse_params "$@"
setup_colors

# Constants, variables
BLD_COMMIT=$(git rev-parse --short HEAD)
readonly BLD_COMMIT
BLD_VERSION_DATE=$(date -u +'%Y%m%d_%H%M%SZ')
readonly BLD_VERSION_DATE

BLD_PUSH=false
BLD_RELEASE=false
BLD_VERSION=unknown
BLD_RELEASE_VERSION=''

readonly BLD_STARTAT=$SECONDS

SYSARCH=$(arch)
readonly SYSARCH

if [[ ${SYSARCH} = "i386" ]]; then
  readonly ARCH="x86_32"
  elif [[ ${SYSARCH} = "aarch64" ]]; then
  readonly ARCH="armv7"
else
  readonly ARCH=${SYSARCH}
fi

OS=$(uname -s)
readonly OS

if [[ -z ${DOCKER_LOCK_VERSION-} && -f "docker-lock-version.txt" ]]; then
  BLD_DOCKER_LOCK_VERSION=$(cat docker-lock-version.txt)
  readonly BLD_DOCKER_LOCK_VERSION
else
  BLD_DOCKER_LOCK_VERSION=${DOCKER_LOCK_VERSION-}
  readonly BLD_DOCKER_LOCK_VERSION
fi

if [[ -n ${BLD_DOCKER_LOCK_VERSION-} && -f "docker-lock.json" ]]; then
  readonly DOCKER_LOCK_URL="https://github.com/safe-waters/docker-lock/releases/download/v${BLD_DOCKER_LOCK_VERSION}/docker-lock_${BLD_DOCKER_LOCK_VERSION}_${OS}_${ARCH}.tar.gz"
  msg "${BLUE}===${NOFORMAT} Using docker-lock.json version ${BLD_DOCKER_LOCK_VERSION} to pin Dockerfile(s)"
  mkdir -p ".docker/cli-plugins"
  curl -fsSL "${DOCKER_LOCK_URL}" | tar -xz -C ".docker/cli-plugins" "docker-lock"
  chmod +x ".docker/cli-plugins/docker-lock"
  .docker/cli-plugins/docker-lock lock rewrite
fi

# Try getting branch from Azure DevOps
readonly AZURE_BRANCH=${BUILD_SOURCEBRANCH-}
BLD_BRANCH=''
BLD_GITBRANCH=${AZURE_BRANCH#refs/heads/}
if [[ -n $BLD_GITBRANCH ]]; then
  BLD_BRANCH=$BLD_GITBRANCH
  msg "${BLUE}===${NOFORMAT} Branch from Azure DevOps: $BLD_BRANCH"
fi

if [[ -z ${BLD_BRANCH} ]]; then
  # Try getting branch from git
  if BLD_GITBRANCH=$(git symbolic-ref --short -q HEAD); then
    BLD_BRANCH=$BLD_GITBRANCH
    msg "${BLUE}===${NOFORMAT} Branch from git: $BLD_BRANCH"
  fi
fi

if [[ $BLD_BRANCH = "develop"
    || $BLD_BRANCH = "main"
    || $BLD_BRANCH = "master"
  || $BLD_BRANCH = "test" ]]; then
  BLD_DOCKER_TAG=$BLD_BRANCH
  BLD_VERSION=${BLD_BRANCH}-${BLD_VERSION_DATE}
  BLD_PUSH=true
  elif [[ "$BLD_BRANCH" =~ release/([0-9]+\.[0-9]+\.[0-9]+.*) ]]; then
  BLD_RELEASE_VERSION=${BASH_REMATCH[1]}
  BLD_DOCKER_TAG=v${BLD_RELEASE_VERSION}
  BLD_VERSION=v${BLD_RELEASE_VERSION}
  BLD_RELEASE=true
  BLD_PUSH=true
else
  BLD_DOCKER_TAG=$BLD_COMMIT
  BLD_VERSION=${BLD_COMMIT}-${BLD_VERSION_DATE}
fi

# Check if we should use a different Dockerfile
if [[ -z $dockerfile ]]; then
  BLD_DOCKERFILE="Dockerfile"
else
  BLD_DOCKERFILE="$dockerfile"
  msg "${BLUE}===${NOFORMAT} Using Dockerfile: $BLD_DOCKERFILE"
fi

# Ensure a configured Docker image name
if [[ -z ${BLD_DOCKER_IMAGE-} ]]; then
  BLD_DIRECTORY=${PWD##*/}
  BLD_DOCKER_IMAGE=${BLD_DIRECTORY,,}
  msg "${ORANGE}===${NOFORMAT} No BLD_DOCKER_IMAGE configured, using this directory name: $BLD_DOCKER_IMAGE"
fi

# Perform release prep if necessary and script is present
if [[ $BLD_RELEASE = "true" && -f "release-prep.bash" ]]; then
  msg "${BLUE}===${NOFORMAT} Running release preparation for version $BLD_RELEASE_VERSION"
  #shellcheck disable=SC1091
  source release-prep.bash
  msg "${GREEN}===${NOFORMAT} Release preparation script complete"
fi

# If there's a specified Docker tag, use it
if [[ -n $dockertag ]]; then
  BLD_DOCKER_TAG="$dockertag"
  msg "${BLUE}===${NOFORMAT} Using specified Docker tag $BLD_DOCKER_TAG"
fi

# Construct complete Docker image and tag from provided values
BLD_DOCKER_LATEST=${BLD_DOCKER_IMAGE}:latest
BLD_DOCKER_IMAGE=${BLD_DOCKER_IMAGE}:${BLD_DOCKER_TAG}
BLD_FULL_DOCKER_IMAGE=${BLD_DOCKER_IMAGE}
BLD_FULL_DOCKER_LATEST=${BLD_DOCKER_LATEST}

if [[ -n ${CR_OWNER-} ]]; then
  msg "${BLUE}===${NOFORMAT} Adding container registry owner $CR_OWNER"
  BLD_FULL_DOCKER_IMAGE=${CR_OWNER}/${BLD_FULL_DOCKER_IMAGE}
  BLD_FULL_DOCKER_LATEST=${CR_OWNER}/${BLD_FULL_DOCKER_LATEST}
fi

if [[ -n ${CR_HOST-} ]]; then
  msg "${BLUE}===${NOFORMAT} Adding container registry host $CR_HOST"
  BLD_FULL_DOCKER_IMAGE=${CR_HOST}/${BLD_FULL_DOCKER_IMAGE}
  BLD_FULL_DOCKER_LATEST=${CR_HOST}/${BLD_FULL_DOCKER_LATEST}
fi

msg "${BLUE}===${NOFORMAT} Building branch $BLD_BRANCH commit $BLD_COMMIT as Docker image $BLD_FULL_DOCKER_IMAGE"
msg "${BLUE}===${NOFORMAT} Image version: $BLD_VERSION"

if [[ $BLD_PUSH = true ]]; then
  docker build -f "$BLD_DOCKERFILE" -t "$BLD_FULL_DOCKER_IMAGE" \
  --build-arg BRANCH="$BLD_BRANCH" \
  --build-arg IMAGE_CREATED="$BLD_VERSION_DATE" \
  --build-arg IMAGE_REVISION="$BLD_COMMIT" \
  --build-arg IMAGE_VERSION="$BLD_VERSION" .
  
  msg "${GREEN}===${NOFORMAT} Docker image built"
  
  dockeruser=${CR_USER-}
  dockerpass=${CR_PASSWORD-}
  
  if [[ -z $dockeruser || -z $dockerpass ]]; then
    msg "${ORANGE}===${NOFORMAT} Not pushing Docker image: username or password not specified"
  else
    msg "${BLUE}===${NOFORMAT} Authenticating..."
    if [[ -z ${CR_HOST-} ]]; then
      echo "$dockerpass" | \
      docker login -u "$dockeruser" --password-stdin || exit $?
    else
      echo "$dockerpass" | \
      docker login -u "$dockeruser" --password-stdin "${CR_HOST-}" || exit $?
    fi
    
    msg "${BLUE}===${NOFORMAT} Pushing image $BLD_FULL_DOCKER_IMAGE"
    docker push "$BLD_FULL_DOCKER_IMAGE"
    
    if [[ $BLD_RELEASE = "true" ]]; then
      msg "${BLUE}===${NOFORMAT} Tagging and pushing $BLD_FULL_DOCKER_LATEST"
      docker tag "$BLD_FULL_DOCKER_IMAGE" "$BLD_FULL_DOCKER_LATEST"
      docker push "$BLD_FULL_DOCKER_LATEST"
    fi
    
    msg "${GREEN}===${NOFORMAT} Docker image pushed"
    
    msg "${BLUE}===${NOFORMAT} Executing logout"
    if [[ -z ${CR_HOST-} ]]; then
      docker logout
    else
      docker logout "${CR_HOST-}"
    fi
  fi
  
  ghcruser=${GHCR_USER-}
  
  if [[ -n $ghcruser ]]; then
    ghcrowner=${GHCR_OWNER-}
    if [[ -z $ghcrowner ]]; then
      ghcrowner=$ghcruser
    fi
    msg "${BLUE}===${NOFORMAT} Pushing image to ghcr.io/${ghcrowner}/${BLD_DOCKER_IMAGE}"
    docker tag "${BLD_FULL_DOCKER_IMAGE}" "ghcr.io/${ghcrowner}/${BLD_DOCKER_IMAGE}"
    echo "$GHCR_PAT" | \
    docker login ghcr.io -u "${ghcruser}" --password-stdin || exit $?
    docker push "ghcr.io/${ghcrowner}/${BLD_DOCKER_IMAGE}"
    
    if [[ $BLD_RELEASE = "true" ]]; then
      msg "${BLUE}===${NOFORMAT} Tagging and pushing ghcr.io/${ghcrowner}/${BLD_DOCKER_LATEST}"
      docker tag "${BLD_FULL_DOCKER_IMAGE}" "ghcr.io/${ghcrowner}/${BLD_DOCKER_LATEST}"
      docker push "ghcr.io/${ghcrowner}/${BLD_DOCKER_LATEST}"
    fi
    
    docker logout ghcr.io
    
    msg "${GREEN}===${NOFORMAT} Docker image pushed to ghcr.io"
  fi
  
  # Perform release publish in the Docker machine if configuration is present
  
  if [[ $BLD_RELEASE = "true" && -f "release-publish.bash" && publish -eq 1 ]]; then
    msg "${BLUE}===${NOFORMAT} Publishing release package for $BLD_RELEASE_VERSION"
    mkdir -p publish
    if [[ -f "release.env" ]]; then
      docker run -i \
      --rm \
      --entrypoint "/app/release-publish.bash" \
      --env-file release.env \
      -e BLD_RELEASE_VERSION="$BLD_RELEASE_VERSION" \
      -v "${PWD}/package:/package" \
      "$BLD_FULL_DOCKER_IMAGE"
    else
      docker run -i \
      --rm \
      --entrypoint "/app/release-publish.bash" \
      -e BLD_RELEASE_VERSION="$BLD_RELEASE_VERSION" \
      -v "${PWD}/package:/package" \
      "$BLD_FULL_DOCKER_IMAGE"
    fi
    msg "${GREEN}===${NOFORMAT} Publish script complete"
  fi
else
  docker build -f "$BLD_DOCKERFILE" -t "$BLD_FULL_DOCKER_IMAGE" \
  --build-arg BRANCH="$BLD_BRANCH" \
  --build-arg IMAGE_CREATED="$BLD_VERSION_DATE" \
  --build-arg IMAGE_REVISION="$BLD_COMMIT" \
  --build-arg IMAGE_VERSION="$BLD_VERSION" \
  --target build .
  msg "${GREEN}===${NOFORMAT} Docker image built"
  msg "${ORANGE}===${NOFORMAT} Not pushing Docker image: branch is not develop, main, test, or versioned release"
fi

msg "${PURPLE}===${NOFORMAT} Build script complete in $((SECONDS - BLD_STARTAT)) seconds."
