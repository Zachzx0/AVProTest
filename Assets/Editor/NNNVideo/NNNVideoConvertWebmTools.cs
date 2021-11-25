using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

public class NNNVideoConvertWebmTools : EditorWindow
{
#if UNITY_EDITOR_WIN
    public static string BatFilePath
    {
        get
        {
            DirectoryInfo assetParent = Directory.GetParent(Application.dataPath);
            return $"{assetParent}/ThirdPartyTools/AVproSupportTools/";
        }
    }

    public static string BatFileName
    {
        get
        {
            return "FFMpegLFPacking.bat";
        }
    }

    protected static bool CheckBat()
    {
        return File.Exists($"{BatFilePath}/{BatFileName}");
    }

    protected static string GetCmd(string oriVideo, string outPutVideo)
    {
        return $"{BatFilePath}/{BatFileName} {oriVideo} {outPutVideo}";
    }

    [MenuItem("Assets/NNNVideo/转换Webm", false)]
    public static void TryConvertWebm()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        DirectoryInfo assetParent = Directory.GetParent(Application.dataPath);

        path = string.Format($"{assetParent.FullName}/{path}");

        UnityDebug.Log($"当前选中物体:[{path}]");

        string[] paths = path.Split('/');
        if (paths.Length <= 0)
        {
            UnityDebug.LogError($"当前物体路径不合法");
            return;
        }

        string fileDirectory = Path.GetDirectoryName(path);
        string fileName = Path.GetFileName(path);
        string fileExtName = Path.GetExtension(path);
        string pureFileName = Path.GetFileNameWithoutExtension(path);

        if (fileExtName != ".webm")
        {
            UnityDebug.LogError($"当前资产不是*.webm");
            return;
        }

        string dstVideoPath = $"{fileDirectory}/{pureFileName}.mp4";

        DoConvert(path, dstVideoPath);
    }

    protected static void DoConvert(string oriVideo, string outPutVideo)
    {
        if (!CheckBat())
        {
            UnityDebug.LogError($"批处理不存在");
            return;
        }

        //创建一个进程
        Process p = new Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
        p.Start();//启动程序

        string strCMD = GetCmd(oriVideo, outPutVideo);

        //向cmd窗口发送输入信息
        p.StandardInput.WriteLine($"cd {BatFilePath}");
        p.StandardInput.WriteLine(strCMD + "&exit");

        p.StandardInput.AutoFlush = true;

        //等待程序执行完退出进程
        p.WaitForExit();
        p.Close();
    }
#endif
}