echo Cleaning up Build Directory
rm -rf ../MatchmakingClient/Builds/StandaloneWindows/

echo Starting Build Process
/Applications/Unity/Hub/Editor/2020.1.5f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -nographics -projectPath ../MatchmakingClient/ -executeMethod MM.Client.CI.Build.PerformBuildStandaloneWindows
echo Ended Build Process