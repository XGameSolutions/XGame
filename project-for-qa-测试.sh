

UNITY_PROJECT_NAME=UnityForQA

dir=$(cd $(dirname $0); pwd)
dir_project=$dir/..
dir_unity=$dir_project/XUnity
dir_unity_project=$dir_unity/$UNITY_PROJECT_NAME

sh sh/check_create_dir.sh $dir_unity
sh sh/check_create_dir.sh $dir_unity_project
sh sh/check_create_dir.sh ${dir_unity_project}/Assets

sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-Packages.git ${dir_unity_project}/Packages
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-ProjectSettings.git ${dir_unity_project}/ProjectSettings
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-ResAB.git ${dir_unity_project}/ResAB
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsEditor.git ${dir_unity_project}/Assets/Editor
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsPlugins.git ${dir_unity_project}/Assets/Plugins
sh sh/clone_or_pull.sh https://github.com/monitor1394/XGame-XClient-AssetsSrc.git ${dir_unity_project}/Assets/Src
