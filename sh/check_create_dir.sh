dir=$1
if [ ! -d "$dir" ];then
    echo mkdir:$dir
    mkdir $dir
fi