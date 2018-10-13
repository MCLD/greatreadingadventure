#!/bin/bash

if [ $# -gt 0 ]; then
  GRAVERSION="$1"
else
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

  if [[ $BRANCH =~ v([0-9]+\.[0-9]+\.[0-9]+.*) || $BRANCH =~ release/([0-9]+\.[0-9]+\.[0-9]+.*) ]]; then
    GRAVERSION=${BASH_REMATCH[1]}
  else
    echo "Git branch is not in the correct format for a version."
  fi
fi

if [ -z "$GRAVERSION" ]; then
  echo "Cannot determine version to build...exiiting."
  exit 1
fi

echo Building release packages for $GRAVERSION...

docker run -it -v $(pwd):/app --workdir /app --rm microsoft/aspnetcore-build:1.1 bash -c "dotnet clean && dotnet restore && dotnet publish -c Release -o /app/release"

mv release GreatReadingAdventure-$GRAVERSION

zip -r9 ../GreatReadingAdventure-$GRAVERSION-noavatars.zip GreatReadingAdventure-$GRAVERSION/

sudo mkdir GreatReadingAdventure-$GRAVERSION/assets

cd GreatReadingAdventure-$GRAVERSION/assets

sudo unzip ../../../defaultavatars-$GRAVERSION.zip

cd ../../

zip -r9 ../GreatReadingAdventure-$GRAVERSION.zip GreatReadingAdventure-$GRAVERSION

du -sch GreatReadingAdventure-$GRAVERSION
du -sch ../GreatReadingAdventure-$GRAVERSION-noavatars.zip
du -sch ../GreatReadingAdventure-$GRAVERSION.zip

