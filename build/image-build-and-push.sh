#!/usr/bin/env bash
set -euo pipefail

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

echo $dockerContextPath
echo $dockerFilePath

docker build -t "jaceks2106/billtracker-api:latest" -f $dockerFilePath $dockerContextPath

docker push "jaceks2106/billtracker-api:latest"