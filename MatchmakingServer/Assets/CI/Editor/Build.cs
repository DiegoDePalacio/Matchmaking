using UnityEditor;
using UnityEngine;

namespace MM.Server.CI
{
    public static class Build
    {
        public static void PerformBuildStandaloneOSX()
        {
            PerformBuild(BuildTarget.StandaloneOSX);
        }

        public static void PerformBuildStandaloneWindows()
        {
            PerformBuild(BuildTarget.StandaloneWindows);
        }
        
        private static void PerformBuild(BuildTarget buildTarget)
        {
            string[] matchmakingServerScenes = {"Assets/Scenes/ServerMain.unity"};
            
            var buildReport = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = matchmakingServerScenes,
                locationPathName = $"./Builds/{buildTarget}/MatchmakingServer",
                target = buildTarget,
                options = BuildOptions.None
            });
            
            Debug.Log($"Result of building process: {buildReport.summary.result}");
        }
    }
}