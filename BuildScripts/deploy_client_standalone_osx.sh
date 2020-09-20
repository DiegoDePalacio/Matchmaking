echo Cleaning up Build Directory
rm -rf ../MatchmakingServer/Builds/StandaloneOSX/

echo Starting Build Process
/Applications/Unity/Hub/Editor/2020.1.5f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -nographics -projectPath ../MatchmakingServer/ -executeMethod MM.Server.CI.Build.PerformBuildStandaloneOSX
echo Ended Build Process