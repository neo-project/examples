#bin/bash

artifactId=ico-template
version=1.0.0
jarFile=$artifactId-$version.jar
neocompilerLocation=../../neo-compiler
neoj=$neocompilerLocation/neoj
neopythonLocation=~/Documents/neo-python/

cd ../
mvn clean install
cp target/$jarFile $neoj/$jarFile

cd $neoj
dotnet run $jarFile

cp $artifactId-$version.avm $neopythonLocation
