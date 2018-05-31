#!/bin/bash

pwd=${PWD##*/}

if [ $pwd = "bin" ]; then
  cd ..
fi

pwd=${PWD##*/}

if [ $pwd = "dev" ]; then
  cd ..
fi

BRANCH=`git name-rev --name-only HEAD`
COMMIT=`git rev-parse --short HEAD`

echo -e "\e[1mBuilding GRA branch \e[96m$BRANCH\e[39m commit \e[93m$COMMIT\e[0m"

if [[ "$BRANCH" == "master" ]]; then
  export TAG="latest";
  export DOCKERFILE="Dockerfile"
elif [[ "$BRANCH" == "develop" ]]; then
  export TAG="develop";
  export DOCKERFILE="dev/Dockerfile"
  echo -e "\e[1m\e[41mAutomatically adding a database migration for $BRANCH branch\e[0m"
else
  export TAG=$COMMIT;
  export DOCKERFILE="Dockerfile"
fi

docker build -f $DOCKERFILE -t mcld/gra:$TAG --build-arg commit="$COMMIT" .
