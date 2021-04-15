#!/usr/bin/env bash
set -euo pipefail

exec 3>&1 # keep near the start of the script
function say () {
  printf "%b\n" "[release] $1" >&3
}

dockerRepoName="jaceks2106/billtracker-api"

version='v0.0.1-dev'

while [[ $# > 0 ]]; do
  case "$1" in

    --version=*) version="${1#*=}"; shift 1;;

    -*) echo "unknown option: $1" >&2; exit 1;;
    *) echo "unknown argument: $1" >&2; exit 1;;
  esac
done

say "Pushing docker images"
docker push "$dockerRepoName:$version"

# Deploying app be restarting, which downloads the 'latest' version each time.
if [[ "$version" =~ ^v[0-9]+\.[0-9]+\.[0-9]+$ ]]; then 
    say "Deploying version"
    az webapp config container set -c "$dockerRepoName:$version" --name billtracker-api-dev --resource-group billtracker-api-dev --subscription a08721a6-b871-437b-9b3b-b64c0beec2b7
fi;