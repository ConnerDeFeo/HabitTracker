rm dev.zip
rm -rf publish
dotnet publish -c Release -r linux-x64 --self-contained false -o ./publish
cp -r .platform ./publish/
cp -r .ebextensions ./publish/
