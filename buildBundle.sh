#! /bin/bash
args=("$@")

args_count=${#args[@]}
linkfile=""
outputPath=""

for (( c=0; c<$args_count; c++ ))
do
x=${args[c]}
echo "argument $x"

if [ $x == "-links" ]; then
    echo "links argument ${args[c+1]}"
    linkfile=${args[c+1]};
fi

if [ $x == "-output" ]; then
    echo "output argument ${args[c+1]}"
    outputPath=${args[c+1]};
fi
done

echo $linkfile
/Applications/Unity/Hub/Editor/2020.3.17f1/Unity.app/Contents/MacOS/Unity -batchMode -nographics -noUpm -executeMethod BuildAssets.LoadScene -LoadScene -links $linkfile -output $outputPath
