#!/bin/bash

BLD_PUSH=false
BLD_BRANCH_FOUND=false
BLD_RELEASE=false
BLD_DOCKERFILE="Dockerfile"
BLD_COMMIT=$(git rev-parse --short HEAD)

if [[ -z $BLD_DOCKER_IMAGE ]]; then
  BLD_DIRECTORY=${PWD##*/}
  BLD_DOCKER_IMAGE=${BLD_DIRECTORY,,}
  echo "=== No BLD_DOCKER_IMAGE configured, using this directory name: $BLD_DOCKER_IMAGE"
fi

if BLD_GITBRANCH=$(git symbolic-ref --short -q HEAD); then
  BLD_BRANCH=$BLD_GITBRANCH
  BLD_BRANCH_FOUND=true
fi

if [[ $BLD_BRANCH_FOUND = "false" ]]; then
  if BLD_GITBRANCH=$(git name-rev --name-only HEAD); then
    # Microsoft VSTS works in detached HEAD state
    BLD_BRANCH=${BLD_GITBRANCH#"remotes/origin/"}
    BLD_BRANCH_FOUND=true
  else
    BLD_BRANCH="unknownbranch"
  fi
fi

if [[ $BLD_BRANCH == "master" ]]; then
  BLD_DOCKER_TAG="latest"
  BLD_PUSH=true
elif [[ $BLD_BRANCH == "develop" ]]; then
  BLD_DOCKER_TAG="develop"
  BLD_PUSH=true
  BLD_DOCKERFILE="dev/Dockerfile"
  echo "=== Using $BLD_DOCKERFILE to add database migration for $BLD_BRANCH build..."
elif [[ $BLD_BRANCH =~ v([0-9]+\.[0-9]+\.[0-9]+.*) || $BLD_BRANCH =~ release/([0-9]+\.[0-9]+\.[0-9]+.*) ]]; then
  BLD_RELEASE_VERSION=${BASH_REMATCH[1]}
  BLD_DOCKER_TAG=v${BLD_RELEASE_VERSION}
  BLD_RELEASE=true
  BLD_PUSH=true
  echo "=== Downloading and decompressing avatar package"
  curl -L -o defaultavatars.zip https://github.com/MCLD/greatreadingadventure/releases/download/v4.0.0/defaultavatars-4.0.0.zip
  mkdir src/GRA.Web/assets
  unzip -q defaultavatars.zip -d src/GRA.Web/assets
  rm defaultavatars.zip
  echo "=== Building release artifacts for $BLD_RELEASE_VERSION"
else
  BLD_DOCKER_TAG=$BLD_COMMIT
fi

if [ $# -gt 0 ]; then
  BLD_DOCKER_TAG="$1"
fi

echo "=== Building branch $BLD_BRANCH commit $BLD_COMMIT as Docker image $BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG"
docker build -f $BLD_DOCKERFILE -t $BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG --build-arg commit="$BLD_COMMIT" --build-arg branch="$BLD_BRANCH" .
if [[ -z $BLD_DOCKER_REPOSITORY ]]; then
  echo '=== Not pushing docker image: no Docker repository configured.'
else
  if [[ $BLD_PUSH = false ]]; then
    echo '=== Not pushing Docker image: branch is not master, develop, or versioned release'
  else
    if [[ -z $BLD_DOCKER_USERNAME || -z $BLD_DOCKER_PASSWORD ]]; then
      echo '=== Not pushing Docker image: username or password not specified'
    else
      if [[ -z $BLD_DOCKER_HOST ]]; then
        echo "=== Pushing Docker image: $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG"
      else
        echo "=== Pushing Docker image: $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG to $BLD_DOCKER_HOST"
      fi
      echo "=== Authenticating..."
      echo "$BLD_DOCKER_PASSWORD" | docker login -u "$BLD_DOCKER_USERNAME" --password-stdin $BLD_DOCKER_HOST || exit $?
      echo "=== Tagging image $BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG as $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG"
      docker tag $BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG
      echo "=== Pushing image $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG"
      docker push $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG
      echo "=== Executing logout!"
      docker logout $BLD_DOCKER_HOST
    fi
  fi
fi

if [[ $BLD_RELEASE = "true" ]]; then
  echo "=== Copying files out of docker image to build release $BLD_RELEASE_VERSION..."
  mkdir GreatReadingAdventure-$BLD_RELEASE_VERSION
  docker run --rm --entrypoint "/bin/bash" -v $(pwd)/GreatReadingAdventure-$BLD_RELEASE_VERSION:/release $BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG -c "cp -a /app/* /release/"

  echo "=== Building GreatReadingAdventure-$BLD_RELEASE_VERSION.zip"
  zip -q -r9 GreatReadingAdventure-$BLD_RELEASE_VERSION.zip GreatReadingAdventure-$BLD_RELEASE_VERSION
  du -sch GreatReadingAdventure-$BLD_RELEASE_VERSION.zip
fi
