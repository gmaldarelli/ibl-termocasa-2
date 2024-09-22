#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p c2461192-cc67-40b6-9f59-ce68a2da55f0 -t
    fi
    cd ../
fi

docker-compose up -d
