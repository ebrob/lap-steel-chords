pushd lap-steel-chord-generator
# This project must be available: https://github.com/eddievelasquez/Bach
Remove-Item .\Bach.Model\ -Recurse -Force
#cp ..\..\Bach\src\Bach.Model .
Copy-Item ..\..\Bach\src\Bach.Model -Destination . -Recurse
pushd lap-steel-chord-generator
dotnet run
popd
popd
pushd lap-steel-chord-displayer
npm install
cp ./node_modules/jquery/dist/jquery.slim.min.* ./scripts
cp ./node_modules/svguitar/dist/svguitar.umd.* ./scripts
cp ./node_modules/bootstrap/dist/js/bootstrap.min.* ./scripts
cp ./node_modules/bootstrap/dist/css/bootstrap.min.* ./styles
popd