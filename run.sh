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

composeFilesPath=$(dir_resolve "$SCRIPT_DIR/build")

webApi=''
runTests=false

while [[ $# > 0 ]]; do
  case "$1" in

    --full-app) webApi="-f $composeFilesPath/docker-compose.webapi.yml"; shift ;;
    --tests) runTests=true; shift ;;

    -*) echo "unknown option: $1" >&2; exit 1;;
    *) echo "unknown argument: $1" >&2; exit 1;;
  esac
done


docker-compose -f "$composeFilesPath/docker-compose.infrastructure.yml" $webApi up -d

if [ "$runTests" = true ] ; then
    ASPNETCORE_ENVIRONMENT=Development
    dotnet test 
fi;