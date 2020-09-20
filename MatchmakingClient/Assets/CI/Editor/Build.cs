using UnityEditor;
using UnityEngine;

namespace MM.Client.CI
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
            string[] matchmakingClientScenes = {"Assets/Scenes/ClientMain.unity"};
            
            var buildReport = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = matchmakingClientScenes,
                locationPathName = $"./Builds/{buildTarget}/MatchmakingClient",
                target = buildTarget,
                options = BuildOptions.None
            });
            
            Debug.Log($"Result of building process: {buildReport.summary.result}");
        }
    }
}