

UNITY_PROJECT_NAME=UnityForCoder

dir=$(cd $(dirname $0); pwd)
dir_project=$dir/..
dir_unity=$dir/../XUnity
dir_unity_project=$dir_unity/$UNITY_PROJECT_NAME

sh sh/check_create_dir.sh $dir_unity
sh sh/check_create_dir.sh $dir_unity_project
sh sh/check_create_dir.sh ${dir_unity_project}/Assets

sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClientLua.git ${dir_project}/XClientLua
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XConfig.git ${dir_project}/XConfig
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XPublicLua.git ${dir_project}/XPublicLua
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XDriver.git ${dir_project}/XDriver
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XServer.git ${dir_project}/XServer
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-Packages.git ${dir_unity_project}/Packages
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-ProjectSettings.git ${dir_unity_project}/ProjectSettings
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-ResAB.git ${dir_unity_project}/ResAB
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsEditor.git ${dir_unity_project}/Assets/Editor
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsPlugins.git ${dir_unity_project}/Assets/Plugins
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsResPublic.git ${dir_unity_project}/Assets/ResPublic
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsResModel.git ${dir_unity_project}/Assets/ResModel
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsResScene.git ${dir_unity_project}/Assets/ResScene
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsResUI.git ${dir_unity_project}/Assets/ResUI
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsSrc.git ${dir_unity_project}/Assets/Src
