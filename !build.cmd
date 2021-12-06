tools\nuget.exe restore .\Bwl.Imaging.sln
tools\!vs-tools !check-vs-projects,!build -m -debug -release Bwl.Imaging.sln
taskkill /f /im MSBuild.exe
pause