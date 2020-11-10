
@echo off

set UNITY_PROJECT_NAME=UnityForCoder

set dir=%~dp0
set dir_project=%dir%..
set dir_unity=%dir_project%/XUnity
set dir_unity_project=%dir_unity%/%UNITY_PROJECT_NAME%

call bat/check_create_dir.bat %dir_unity%
call bat/check_create_dir.bat %dir_unity_project%
call bat/check_create_dir.bat %dir_unity_project%/Assets

call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClientLua.git %dir_project%/XClientLua
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XConfig.git %dir_project%/XConfig
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XPublicLua.git %dir_project%/XPublicLua
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XDriver.git %dir_project%/XDriver
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XServer.git %dir_project%/XServer
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-Packages.git %dir_unity_project%/Packages
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-ProjectSettings.git %dir_unity_project%/ProjectSettings
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-ResAB.git %dir_unity_project%/ResAB
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsEditor.git %dir_unity_project%/Assets/Editor
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsPlugins.git %dir_unity_project%/Assets/Plugins
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsResPublic.git %dir_unity_project%/Assets/ResPublic
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsResModel.git %dir_unity_project%/Assets/ResModel
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsResScene.git %dir_unity_project%/Assets/ResScene
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsResUI.git %dir_unity_project%/Assets/ResUI
call bat/clone_or_pull.bat https://github.com/monitor1394/XGame-XClient-AssetsSrc.git %dir_unity_project%/Assets/Src

pause