#!/bin/bash

PROJECT="mcld/gra"

COMMIT=`git rev-parse --short HEAD`
DOCKERFILE="Dockerfile"

if GITBRANCH=$(git symbolic-ref --short -q HEAD); then
  BRANCH=$GITBRANCH
else
  if GITBRANCH=$(git name-rev --name-only HEAD); then
	# Microsoft VSTS works in detached HEAD state
	BRANCH=${GITBRANCH#"remotes/origin/"}
  else
    BRANCH="no-branch"
  fi
fi

if [[ $BRANCH == "master" ]]; then
  TAG="latest"
elif [[ $BRANCH == "develop" ]]; then
  echo "Adding database migration for $BRANCH build..."
  TAG="develop"
  DOCKERFILE="dev/Dockerfile"
elif [[ $BRANCH =~ (v[0-9]+\.[0-9]+\.[0-9]+.*) ]]; then
  TAG=$BRANCH
else
  TAG=$COMMIT
fi

if [ $# -gt 0 ]; then
  TAG="$1"
fi

echo -e "Building branch $BRANCH commit $COMMIT tagged $TAG"

docker build -f $DOCKERFILE -t $PROJECT:$TAG --build-arg commit="$COMMIT" --build-arg branch="$BRANCH" .
