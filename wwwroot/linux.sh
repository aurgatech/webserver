#!/bin/bash
#set -e

BASEDIR=$(dirname "$0")
cd $BASEDIR
BASEDIR=`pwd`
NVM=`command -v nvm`
NPM=`command -v npm`

# check if nvm exists
if [ "$NPM" = "" ]; then
    if [ "$NVM" = "" ]; then
        echo "nvm is not installed"
        rm -Rf nvm
        git clone --branch v0.40.0 --depth 1 https://github.com/nvm-sh/nvm.git
        cd nvm
        bash install.sh

        source ~/.bashrc
        cd ..
        rm -Rf nvm
    fi

    echo "installing nodejs..."
    nvm install 16
fi

set -e

npm install
npx vite build
rm -Rf ../app/wwwroot
mkdir -p ../app/wwwroot
cp -Rf dist/* ../app/wwwroot
