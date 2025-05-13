using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class RunExeFromEditor
{
    [MenuItem("工具/运行外部EXE（选择文件）")]
    public static void RunExternalExe()
    {
        // 弹出文件选择面板，限制为 .exe 文件
        string exePath = EditorUtility.OpenFilePanel("选择要运行的 EXE 文件", "", "exe");

        if (string.IsNullOrEmpty(exePath))
        {
            UnityEngine.Debug.LogWarning("未选择任何 EXE 文件。");
            return;
        }

        if (!File.Exists(exePath))
        {
            UnityEngine.Debug.LogError("文件不存在: " + exePath);
            return;
        }

        // 创建进程启动信息
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            // Arguments = "--your-args", // 如需传入参数，可设置这里
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            Process process = new Process
            {
                StartInfo = startInfo
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    UnityEngine.Debug.Log("[EXE输出] " + args.Data);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    UnityEngine.Debug.LogError("[EXE错误] " + args.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            UnityEngine.Debug.Log("已启动 EXE，进程ID：" + process.Id);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("运行 EXE 失败: " + ex.Message);
        }
    }
}
