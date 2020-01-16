#!/usr/bin/env bash

# @(#) docker-build.bash - shell script for building docker images
#
# Copyright (C) 2018 Maricopa County Library District, All Rights Reserved.
# Releaesd under The MIT License
# https://github.com/MCLD/greatreadingadventure/blob/develop/LICENSE

set -o errexit
set -o pipefail

readonly BLD_COMMIT=$(git rev-parse --short HEAD)
readonly BLD_VERSION_DATE=$(date -u +'%Y%m%d_%H%M%SZ')
readonly BLD_DATE=$(date -u +'%Y-%m-%dT%H:%M:%SZ')

BLD_PUSH=false
BLD_RELEASE=false
BLD_VERSION=unknown

# Branch information

# Try getting branch from Travis environment
if [[ -n $TRAVIS_PULL_REQUEST && $TRAVIS_PULL_REQUEST != false ]]; then
  BLD_BRANCH="PR-$TRAVIS_PULL_REQUEST"
  echo "=== Branch from Travis PR: $BLD_BRANCH"
else
  if [[ -n $TRAVIS_BRANCH ]]; then
    BLD_BRANCH=$TRAVIS_BRANCH
    echo "=== Branch from Travis Push: $BLD_BRANCH"
  fi
fi

if [[ -z $BLD_BRANCH ]]; then
  # Try getting branch from Azure DevOps
  BLD_GITBRANCH=${BUILD_SOURCEBRANCH#refs/heads/}
  if [[ -n $BLD_GITBRANCH ]]; then
    BLD_BRANCH=$BLD_GITBRANCH
    echo "=== Branch from Azure DevOps: $BLD_BRANCH"
  fi
fi

if [[ -z $BLD_BRANCH ]]; then
  # Try getting branch from git
  if BLD_GITBRANCH=$(git symbolic-ref --short -q HEAD); then
    BLD_BRANCH=$BLD_GITBRANCH 
    echo "=== Branch from git: $BLD_BRANCH"
  fi
fi

if [[ -z $BLD_BRANCH ]]; then
  BLD_BRANCH="unknown-branch"
fi

# Supplemental settings based on detected branch

if [[ $BLD_BRANCH = "master" ]]; then
  BLD_DOCKER_TAG="latest"
  BLD_VERSION=${BLD_BRANCH}-${BLD_VERSION_DATE}
  BLD_PUSH=true
  BLD_RELEASE=true
elif [[ $BLD_BRANCH = "develop" ]]; then
  BLD_DOCKER_TAG="develop"
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

if [[ -z $BLD_DOCKERFILE ]]; then
  BLD_DOCKERFILE_CHECK=Dockerfile_${BLD_DOCKER_IMAGE}
  if [[ -n $BLD_DOCKER_IMAGE && -f $BLD_DOCKERFILE_CHECK ]]; then
    BLD_DOCKERFILE=$BLD_DOCKERFILE_CHECK
  else
    BLD_DOCKERFILE="Dockerfile"
  fi
fi

# Ensure a configured Docker image name

if [[ -z $BLD_DOCKER_IMAGE ]]; then
  BLD_DIRECTORY=${PWD##*/}
  BLD_DOCKER_IMAGE=${BLD_DIRECTORY,,}
  echo "=== No BLD_DOCKER_IMAGE configured, using this directory name: $BLD_DOCKER_IMAGE"
fi

# Perform release prep if necessary and script is present

if [[ $BLD_RELEASE = "true" && -f "release-prep.bash" ]]; then
  echo "=== Running release preparation for version $BLD_RELEASE_VERSION"
  #shellcheck disable=SC1091
  source release-prep.bash
fi

# If there's a commandline parameter use it as the Docker tag

if [ $# -gt 0 ]; then
  BLD_DOCKER_TAG="$1"
fi

# Construct complete Docker image and tag from provided values

BLD_FULL_DOCKER_IMAGE=${BLD_DOCKER_IMAGE}:${BLD_DOCKER_TAG}

if [[ -n $BLD_DOCKER_REPOSITORY ]]; then
  BLD_FULL_DOCKER_IMAGE=${BLD_DOCKER_REPOSITORY}/${BLD_FULL_DOCKER_IMAGE}
fi

if [[ -n $BLD_DOCKER_HOST ]]; then
  BLD_FULL_DOCKER_IMAGE=${BLD_DOCKER_HOST}/${BLD_FULL_DOCKER_IMAGE}
fi

echo "=== Building branch $BLD_BRANCH commit $BLD_COMMIT as Docker image $BLD_FULL_DOCKER_IMAGE"
echo "=== Image version: $BLD_VERSION"

if [[ $BLD_PUSH = true ]]; then
  docker build -f "$BLD_DOCKERFILE" -t "$BLD_FULL_DOCKER_IMAGE" \
    --build-arg BRANCH="$BLD_BRANCH" \
    --build-arg IMAGE_CREATED="$BLD_DATE" \
    --build-arg IMAGE_REVISION="$BLD_COMMIT" \
    --build-arg IMAGE_VERSION="$BLD_VERSION" .

  if [[ -z $BLD_DOCKER_USERNAME || -z $BLD_DOCKER_PASSWORD ]]; then
    echo '=== Not pushing Docker image: username or password not specified'
  else
    echo "=== Authenticating..."
    if [[ -z $BLD_DOCKER_HOST ]]; then
      echo "$BLD_DOCKER_PASSWORD" | \
      docker login -u "$BLD_DOCKER_USERNAME" --password-stdin || exit $?
    else
      echo "$BLD_DOCKER_PASSWORD" | \
      docker login -u "$BLD_DOCKER_USERNAME" --password-stdin "$BLD_DOCKER_HOST" || exit $?
    fi

    echo "=== Pushing image $BLD_FULL_DOCKER_IMAGE"
    docker push "$BLD_FULL_DOCKER_IMAGE"

    echo "=== Executing logout"
    if [[ -z $BLD_DOCKER_HOST ]]; then
      echo "$BLD_DOCKER_PASSWORD" | \
      docker logout
    else
      echo "$BLD_DOCKER_PASSWORD" | \
      docker logout "$BLD_DOCKER_HOST"
    fi
  fi
else
  docker build -f "$BLD_DOCKERFILE" -t "$BLD_FULL_DOCKER_IMAGE" --target build-stage .
  echo '=== Not pushing Docker image: branch is not master, develop, or versioned release'
fi

# Perform release publish in the Docker machine if configuration is present

if [[ $BLD_RELEASE = "true" && -f "release-publish.bash" ]]; then
  echo "=== Publishing release package for $BLD_RELEASE_VERSION"
  if [[ -f "release.env" ]]; then
    docker run -it \
      --rm \
      --entrypoint "/app/release-publish.bash" \
      --env-file release.env \
      -e BLD_RELEASE_VERSION="$BLD_RELEASE_VERSION" \
      "$BLD_FULL_DOCKER_IMAGE"
  else
    docker run -it \
      --rm \
      --entrypoint "/app/release-publish.bash" \
      -e BLD_RELEASE_VERSION="$BLD_RELEASE_VERSION" \
      "$BLD_FULL_DOCKER_IMAGE"
  fi
fi
