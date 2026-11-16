#!/bin/sh

# save in .git\hooks

docker run --rm -v "$(pwd):/repo" zricethezav/gitleaks:latest protect --staged --source /repo -v