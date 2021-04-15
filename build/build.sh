#!/usr/bin/env bash
set -euo pipefail

exec 3>&1 # keep near the start of the script
function say () {
  printf "%b\n" "[build] $1" >&3
}

# https://stackoverflow.com/a/20901614
function dir_resolve() {
  local dir=`dirname "$1"`
  local file=`basename "$1"`
  if [ "$file" = ".." ]; then
    pushd "$1" &>/dev/null || return $? # On error, return error code
    echo "`pwd -P`" # output full, link-resolved path with filename
    popd &> /dev/null
  else
    pushd "$dir" &>/dev/null || return $? # On error, return error code
    echo "`pwd -P`/$file" # output full, link-resolved path with filename
    popd &> /dev/null
  fi
}

# The full path to this script. https://stackoverflow.com/a/246128
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

dockerContextPath=$(dir_resolve "$SCRIPT_DIR/../src")
dockerFilePath=$(dir_resolve "$SCRIPT_DIR/../src/BillTracker.Api/Dockerfile")

dockerRepoName="jaceks2106/billtracker-api"

version='v0.0.1-dev'

while [[ $# > 0 ]]; do
  case "$1" in

    --version=*) version="${1#*=}"; shift 1;;

    -*) echo "unknown option: $1" >&2; exit 1;;
    *) echo "unknown argument: $1" >&2; exit 1;;
  esac
done

say "Version: '$version'"

# do not create any docker tag if version is wrong, e.g. 'master', '123'
versionTagOption="-t $dockerRepoName:$version"
if [[ ! "$version" =~ ^v[0-9]+\.[0-9]+\. ]]; then
  versionTagOption=''
fi;

say "Building Docker image"
docker build $versionTagOption -f $dockerFilePath $dockerContextPath
