@echo off

set UNITY_PROJECT_NAME=UnityForQA

set dir=%~dp0
set dir_unity=%dir%../XUnity
set dir_unity_project=%dir_unity%/%UNITY_PROJECT_NAME%

call bat/check_create_dir.bat %dir_unity%
call bat/check_create_dir.bat %dir_unity_project%
call bat/check_create_dir.bat %dir_unity_project%/Assets

call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-Packages.git %dir_unity_project%/Packages
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-ProjectSettings.git %dir_unity_project%/ProjectSettings
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-ResAB.git %dir_unity_project%/ResAB
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsEditor.git %dir_unity_project%/Assets/Editor
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsPlugins.git %dir_unity_project%/Assets/Plugins
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsSrc.git %dir_unity_project%/Assets/Src

pause