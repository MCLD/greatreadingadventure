#!/bin/bash

PUSH=false
BRANCH_FOUND=false
DOCKERFILE="Dockerfile"
COMMIT=$(git rev-parse --short HEAD)

if GITBRANCH=$(git symbolic-ref --short -q HEAD); then
  BRANCH=$GITBRANCH
  BRANCH_FOUND=true
fi

if [[ $BRANCH_FOUND = "false" ]]; then
  if GITBRANCH=$(git name-rev --name-only HEAD); then
  	# Microsoft VSTS works in detached HEAD state
  	BRANCH=${GITBRANCH#"remotes/origin/"}
	BRANCH_FOUND=true
  else
    BRANCH="unknownbranch"
  fi
fi

if [[ $BRANCH == "master" ]]; then
  DOCKER_TAG="latest"
  PUSH=true
elif [[ $BRANCH == "develop" ]]; then
  DOCKER_TAG="develop"
  PUSH=true
  DOCKERFILE="dev/Dockerfile"
  echo "Using $DOCKERFILE to add database migration for $BRANCH build..."
elif [[ $BRANCH =~ v([0-9]+\.[0-9]+\.[0-9]+.*) || $BRANCH =~ release/([0-9]+\.[0-9]+\.[0-9]+.*) ]]; then
  DOCKER_TAG=v${BASH_REMATCH[1]}
  PUSH=true
else
  DOCKER_TAG=$COMMIT
fi

if [ $# -gt 0 ]; then
  DOCKER_TAG="$1"
fi

if [[ -z $DOCKER_REPOSITORY ]]; then
  if [[ -z $DOCKER_IMAGE ]]; then
    echo -e "Building branch $BRANCH commit $COMMIT as Docker image $DOCKER_TAG"
    docker build -f $DOCKERFILE -t $DOCKER_TAG --build-arg commit="$COMMIT" --build-arg branch="$BRANCH" .
  else
    echo -e "Building branch $BRANCH commit $COMMIT as Docker image $DOCKER_IMAGE:$DOCKER_TAG"
    docker build -f $DOCKERFILE -t $DOCKER_IMAGE:$DOCKER_TAG --build-arg commit="$COMMIT" --build-arg branch="$BRANCH" .
  fi
  echo 'Not pushing Docker image: no Docker repository configured'
else
  echo -e "Building branch $BRANCH commit $COMMIT as Docker image $DOCKER_REPOSITORY/$DOCKER_IMAGE:$DOCKER_TAG"
  docker build -f $DOCKERFILE -t $DOCKER_REPOSITORY/$DOCKER_IMAGE:$DOCKER_TAG --build-arg commit="$COMMIT" --build-arg branch="$BRANCH" .
  if [[ -z $DOCKER_USERNAME || -z $DOCKER_PASSWORD ]]; then
    echo 'Not pushing Docker image: username or password not specified'
  else
    if [[ $PUSH = true ]]; then
      echo "$DOCKER_PASSWORD" | docker login -u "$DOCKER_USERNAME" --password-stdin $DOCKER_HOST
      echo "Pushing Docker image: $DOCKER_REPOSITORY/$DOCKER_IMAGE:$DOCKER_TAG"
      docker push $DOCKER_REPOSITORY/$DOCKER_IMAGE:$DOCKER_TAG
      docker logout $DOCKER_REPOSITORY
    else
      echo 'Not pushing Docker image: branch is not master, develop, or versioned release'
    fi
  fi
fi
