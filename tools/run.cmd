@echo off
pushd "%~d0%~p0..\target"
node %*
popd
