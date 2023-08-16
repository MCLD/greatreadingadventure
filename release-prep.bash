#!/usr/bin/env bash

set -Eeuo pipefail
trap cleanup SIGINT SIGTERM ERR EXIT

script_dir=$(cd "$(dirname "${BASH_SOURCE[0]}")" &>/dev/null && pwd -P)

usage() {
  cat <<EOF
Usage: $(basename "${BASH_SOURCE[0]}") [-h] [-v]

Script description here.

Available options:

-h, --help      Print this help and exit
-v, --verbose   Print script debug info
EOF
    exit
}

cleanup() {
    trap - SIGINT SIGTERM ERR EXIT
    # script cleanup here
}

setup_colors() {
    if [[ -t 2 ]] && [[ -z "${NO_COLOR-}" ]] && [[ "${TERM-}" != "dumb" ]]; then
        NOFORMAT='\033[0m' RED='\033[0;31m' GREEN='\033[0;32m' ORANGE='\033[0;33m' BLUE='\033[0;34m' PURPLE='\033[0;35m' CYAN='\033[0;36m' YELLOW='\033[1;33m'
    else
        NOFORMAT='' RED='' GREEN='' ORANGE='' BLUE='' PURPLE='' CYAN='' YELLOW=''
    fi
}

msg() {
    echo >&2 -e "${1-}"
}

die() {
    local msg=$1
    local code=${2-1} # default exit status 1
    msg "$msg"
    exit "$code"
}

parse_params() {
    # default values of variables set from params
    publish=0
    
    while :; do
        case "${1-}" in
            -h | --help) usage ;;
            -v | --verbose) set -x ;;
            --no-color) NO_COLOR=1 ;;
            -p | --publish)
                publish=1
            ;;
            -?*) die "Unknown option: $1" ;;
            *) break ;;
        esac
        shift
    done
    
    return 0
}

parse_params "$@"
setup_colors

# script logic here

readonly PUB_STARTAT=$SECONDS
readonly AVATAR_ARCH_NAME=v4.2.2.tar.gz

if [[ publish -eq 1 ]]; then
    msg "${BLUE}===${NOFORMAT} Downloading and decompressing avatar package: ${AVATAR_ARCH_NAME}"
    
    curl -Lo defaultavatars.tgz https://github.com/MCLD/gra-avatars/archive/${AVATAR_ARCH_NAME}
    readonly AVATAR_ARCH_SIZE=$(du -sh --apparent-size defaultavatars.tgz |cut -f1)
    
    mkdir -p src/GRA.Web/assets/defaultavatars
    tar -xzf defaultavatars.tgz --strip-components=1 -C src/GRA.Web/assets/defaultavatars
    readonly AVATAR_DIR_SIZE=$(du -sh --apparent-size src/GRA.Web/assets/defaultavatars |cut -f1)
    rm defaultavatars.tgz
    
    msg "${PURPLE}===${NOFORMAT} Downloaded and decompressed ${AVATAR_ARCH_SIZE} archive to ${AVATAR_DIR_SIZE} of files in $((SECONDS - PUB_STARTAT)) seconds."
else
    msg "${RED}===${NOFORMAT} Missing -p flag, not running release prep"
fi
