@echo off
npm install --global --production windows-build-tools
npm install --global node-gyp

call "%~d0%~p0env.cmd"