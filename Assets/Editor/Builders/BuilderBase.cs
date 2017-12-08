using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Editor.Builders
{
    public class BuilderBase
    {
        /// <summary>
        /// gets the list of scene file names that are in the Scenes/Gameplay directory
        /// </summary>
        public static List<string> GetGameplayScenes()
        {
            var sceneFileNames = new List<string>();

            var gameplayScenesAbsolutePath = Path.Combine(Application.dataPath, "Scenes/Gameplay/");
            DirectoryInfo di = new DirectoryInfo(Application.dataPath);

            // find all *.unity files in directory
            var fileSystemInfos = di.GetFileSystemInfos("*.unity");
            Debug.Log("Found " + fileSystemInfos.Length + " Gameplay scenes in " + gameplayScenesAbsolutePath);            
            
            foreach(var fsi in fileSystemInfos)
                sceneFileNames.Add(Path.Combine("Scenes/Gameplay/", fsi.Name));

            return sceneFileNames;
        }

        /// <summary>
        /// gets list of scene file names that are in the Scenes/Client directory
        /// </summary>
        /// <returns></returns>
        public static List<string> GetClientScenes()
        {
            var sceneFileNames = new List<string>();

            var gameplayScenesAbsolutePath = Path.Combine(Application.dataPath, "Scenes/Client/");
            DirectoryInfo di = new DirectoryInfo(Application.dataPath);

            // find all *.unity files in directory
            var fileSystemInfos = di.GetFileSystemInfos("*.unity");
            Debug.Log("Found " + fileSystemInfos.Length + " Gameplay scenes in " + gameplayScenesAbsolutePath);            
            
            foreach(var fsi in fileSystemInfos)
                sceneFileNames.Add(Path.Combine("Scenes/Client/", fsi.Name));

            return sceneFileNames;
        }
    }
}