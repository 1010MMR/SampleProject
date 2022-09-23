using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class ExcelConvert
{
    [MenuItem("Tools/Table/Excel To Bytes (x64)", false, 1)]
    private static void ExcelToBytesMenu_x64()
    {
        Process process = new Process();
        ProcessStartInfo info = new ProcessStartInfo(ExcelToBytesProperty.GetExcelToBytesToolPath_x64, string.Format("{0},{1},{2}", ExcelToBytesProperty.GetExcelPath, 
                                                                                                                                           ExcelToBytesProperty.GetBytePath, 
                                                                                                                                           ExcelToBytesProperty.GetCSPath));

        process.StartInfo = info;
        process.Start();
    }

    [MenuItem("Tools/Table/Excel To Bytes (x86)", false, 2)]
    private static void ExcelToBytesMenu_x86()
    {
        Process process = new Process();
        ProcessStartInfo info = new ProcessStartInfo(ExcelToBytesProperty.GetExcelToBytesToolPath_x86, string.Format("{0},{1},{2}", ExcelToBytesProperty.GetExcelPath,
                                                                                                                                           ExcelToBytesProperty.GetBytePath,
                                                                                                                                           ExcelToBytesProperty.GetCSPath));

        process.StartInfo = info;
        process.Start();
    }
}