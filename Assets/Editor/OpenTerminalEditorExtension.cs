using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class OpenTerminalEditorExtension
{
    [MenuItem("Assets/Open Terminal")]
    public static void OpenTerminal()
    {
        // path defaults to project root
        string path = Application.dataPath;
        
        // if right clicked something in project view, open terminal in the directory of whatever they selected
        if (Selection.activeObject != null)
        {
            UnityEngine.Debug.Log(Selection.activeObject.name);
        }

        Process p = new Process();
#if UNITY_EDITOR_OSX
        p.StartInfo.FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
        p.StartInfo.UseShellExecute = false ;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
//        p.StartInfo.FileName ="open";
//        p.StartInfo.Arguments = "Talk.command";
        p.StartInfo.WorkingDirectory=path;
//        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.StandardInput.WriteLine("cd " + path);
            
            
            
            ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash");
            startInfo.WorkingDirectory = "/";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
 
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
 
//            process.StandardInput.WriteLine("say " + command);
//            process.StandardInput.WriteLine("exit");
            process.StandardInput.WriteLine("xterm -e &");
            process.StandardInput.Flush();
 
            string line = process.StandardOutput.ReadLine();
       
            process.WaitForExit();
#elif UNITY_EDITOR
        p.StartInfo.FileName = "cmd.exe";
        //p.StartInfo.Arguments = @"/c D:\\pdf2xml";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardInput = true;
        p.Start();
        p.StandardInput.WriteLine(@"cd "+ path);
#endif
    }
}