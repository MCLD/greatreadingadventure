#!/bin/bash

PROJECT="gra"

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
  TAG="latest"
  PUSH=true
elif [[ $BRANCH == "develop" ]]; then
  echo "Adding database migration for $BRANCH build..."
  TAG="develop"
  PUSH=true
  DOCKERFILE="dev/Dockerfile"
elif [[ $BRANCH =~ v([0-9]+\.[0-9]+\.[0-9]+.*) || $BRANCH =~ release/([0-9]+\.[0-9]+\.[0-9]+.*) ]]; then
  TAG=v${BASH_REMATCH[1]}
  PUSH=true
else
  TAG=$COMMIT
fi

if [ $# -gt 0 ]; then
  TAG="$1"
fi


if [[ -z $DOCKER_PREFIX ]]; then
  echo -e "Building branch $BRANCH commit $COMMIT as Docker image $PROJECT:$TAG"
  docker build -f $DOCKERFILE -t $PROJECT:$TAG --build-arg commit="$COMMIT" --build-arg branch="$BRANCH" .
  echo 'Not pushing Docker image: no Docker prefix configured'
else
  echo -e "Building branch $BRANCH commit $COMMIT as Docker image $DOCKER_PREFIX/$PROJECT:$TAG"
  docker build -f $DOCKERFILE -t $DOCKER_PREFIX/$PROJECT:$TAG --build-arg commit="$COMMIT" --build-arg branch="$BRANCH" .
  if [[ -z $DOCKER_USERNAME || -z $DOCKER_PASSWORD ]]; then
    echo 'Not pushing Docker image: username or password not specified'
  else
    if [[ $PUSH = true ]]; then
      echo "$DOCKER_PASSWORD" | docker login -u "$DOCKER_USERNAME" --password-stdin $DOCKER_PREFIX
      echo "Pushing Docker image: $DOCKER_PREFIX/$PROJECT:$TAG"
      docker push $DOCKER_PREFIX/$PROJECT:$TAG
      docker logout $DOCKER_PREFIX
    else
      echo 'Not pushing Docker image: branch is not master, develop, or versioned release'
    fi
  fi
fi
