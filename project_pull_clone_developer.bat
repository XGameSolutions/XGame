
@echo off

set UNITY_PROJECT_NAME=UnityForCoder

set dir=%~dp0
set dir_unity_project=%dir%/../XUnity/UnityForCoder

set buildtarget=StandaloneWindow64
set repo_group=developer

cd %dir%
python3 scripts/tool_gitpull.py %dir_unity_project% %buildtarget% %repo_group%

pause