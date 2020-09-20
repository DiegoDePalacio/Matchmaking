echo Cleaning up Build Directory
rm -rf ../MatchmakingClient/Builds/StandaloneOSX/

echo Starting Build Process
/Applications/Unity/Hub/Editor/2020.1.5f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -nographics -projectPath ../MatchmakingClient/ -executeMethod MM.Client.CI.Build.PerformBuildStandaloneOSX
echo Ended Build Process