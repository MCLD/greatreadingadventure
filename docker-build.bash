#!/bin/bash

BLD_PUSH=false
BLD_BRANCH_FOUND=false
BLD_DOCKERFILE="Dockerfile"
BLD_COMMIT=$(git rev-parse --short HEAD)

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
  echo "Using $BLD_DOCKERFILE to add database migration for $BLD_BRANCH build..."
elif [[ $BLD_BRANCH =~ v([0-9]+\.[0-9]+\.[0-9]+.*) || $BLD_BRANCH =~ release/([0-9]+\.[0-9]+\.[0-9]+.*) ]]; then
  BLD_DOCKER_TAG=v${BASH_REMATCH[1]}
  BLD_PUSH=true
else
  BLD_DOCKER_TAG=$BLD_COMMIT
fi

if [ $# -gt 0 ]; then
  BLD_DOCKER_TAG="$1"
fi

if [[ -z $BLD_DOCKER_IMAGE ]]; then
  echo -e "Building branch: $BLD_BRANCH commit: $BLD_COMMIT as Docker image: $BLD_DOCKER_IMAGE/$BLD_DOCKER_TAG"
  docker build -f $BLD_DOCKERFILE -t $BLD_DOCKER_TAG --build-arg commit="$BLD_COMMIT" --build-arg branch="$BLD_BRANCH" .
else
  echo -e "Building branch $BLD_BRANCH commit $BLD_COMMIT as Docker image $BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG"
  docker build -f $BLD_DOCKERFILE -t $BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG --build-arg commit="$BLD_COMMIT" --build-arg branch="$BLD_BRANCH" .
  if [[ -z $BLD_DOCKER_REPOSITORY ]]; then
    echo 'Not pushing docker image: no Docker repository configured.'
  else
    if [[ $BLD_PUSH = false ]]; then
      echo 'Not pushing Docker image: branch is not master, develop, or versioned release'
	else
	  if [[ -z $BLD_DOCKER_USERNAME || -z $BLD_DOCKER_PASSWORD ]]; then
	    echo 'Not pushing Docker image: username or password not specified'
	  else
	    if [[ -z $BLD_DOCKER_HOST ]]; then
	      echo "Pushing Docker image: $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG"
		else
          echo "Pushing Docker image: $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG to $BLD_DOCKER_HOST"
		fi
		echo "Authenticating..."
        echo "$BLD_DOCKER_PASSWORD" | docker login -u "$BLD_DOCKER_USERNAME" --password-stdin $BLD_DOCKER_HOST || exit $?  
		echo "Tagging image $BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG as $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG"
	    docker tag $BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG
		echo "Pushing image $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG"
        docker push $BLD_DOCKER_REPOSITORY/$BLD_DOCKER_IMAGE:$BLD_DOCKER_TAG
		echo "Executing logout!"
        docker logout $BLD_DOCKER_HOST
	  fi
	fi
  fi
fi
