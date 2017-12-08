using System.Runtime.CompilerServices;
using UnityEditor;

namespace Editor.Builders
{
    public class MacServerBuilder : BuilderBase
    {
        static void ProductionBuild()
        {
            BuildPlayerOptions bpo = new BuildPlayerOptions();
            bpo.locationPathName = "Builds/MacServer";
            bpo.target = BuildTarget.StandaloneOSXIntel64;
            bpo.options = BuildOptions.EnableHeadlessMode; // headless mode for server

            // get gameplay scenes... they should all be included for client+server
            var scenesForBuild = GetGameplayScenes();
            
            // configure startup scene for the server build.
            scenesForBuild.Insert(0, "Assets/Scenes/ServerStart.unity");
            
            // assign scenes to build
            bpo.scenes = scenesForBuild.ToArray();

            // run the build
            BuildPipeline.BuildPlayer(bpo);
        }
    }
}