using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.Builders
{
    public class WinBuilder : BuilderBase
    {
        static void ProductionBuild()
        {
            BuildPlayerOptions bpo = new BuildPlayerOptions();
            bpo.locationPathName = "Builds/Win64";
            bpo.target = BuildTarget.StandaloneWindows64;
            bpo.options = BuildOptions.None;

            // include all gameplay scenes in build
            var scenesForBuild = GetGameplayScenes();

            // include all client scenes in build
            scenesForBuild.AddRange(GetClientScenes());

            // find the ClientStart scene (it should be included with GetClientScenes)
            var clientStartScene = scenesForBuild.FirstOrDefault(s => s.Equals("Assets/Scenes/ClientStart.unity",
                StringComparison.InvariantCultureIgnoreCase));

            if (clientStartScene == null)
                throw new FileNotFoundException("Could not find the ClientStart scene!");

            // configure startup scene for the client. Remove it from list, then insert as first element in the list.
            scenesForBuild.Remove(clientStartScene);
            scenesForBuild.Insert(0, clientStartScene);

            // build with all scenes that the zone server should be able to run
            bpo.scenes = scenesForBuild.ToArray();

            string error = BuildPipeline.BuildPlayer(bpo);
            if (!string.IsNullOrEmpty(error))
                Debug.Log(error);
        }
    }
}