
#!/bin/bash

dir=$(cd $(dirname $0); pwd)
dir_unity_project=$dir/../XUnity/UnityForCoder

buildtarget=iOS
repo_group=developer

cd $dir
python3 scripts/tool_gitpull.py $dir_unity_project $buildtarget $repo_group
