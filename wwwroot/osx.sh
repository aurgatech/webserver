#!/bin/bash
BASEDIR=$(dirname "$0")
cd $BASEDIR
BASEDIR=`pwd`

set -e
npm install
npx vite build
rm -Rf ../app/wwwroot
mkdir -p ../app/wwwroot
cp -Rf dist/* ../app/wwwroot
