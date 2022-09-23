using System.IO;
using UnityEngine;

public static class ExcelToBytesProperty
{
    private static string GetParentFullName { get { return Directory.GetParent(System.Environment.CurrentDirectory).FullName; } }

    /// <summary> ExcelToBytesTool의 경로 (x64). </summary>
    public static string GetExcelToBytesToolPath_x64 { get { return string.Format("{0}/ExcelToBytes/x64/ExcelToBytesTool.exe", GetParentFullName); } }
    /// <summary> ExcelToBytesTool의 경로 (x86). </summary>
    public static string GetExcelToBytesToolPath_x86 { get { return string.Format("{0}/ExcelToBytes/x86/ExcelToBytesTool.exe", GetParentFullName); } }

    /// <summary> ExcelDBUploadTool의 경로 (x64). </summary>
    public static string GetExcelDBUploaderToolPath_x64 { get { return string.Format("{0}/ExcelDBUpload/x64/ExcelDBUploadTool.exe", GetParentFullName); } }
    /// <summary> ExcelDBUploadTool의 경로 (x86). </summary>
    public static string GetExcelDBUploaderToolPath_x86 { get { return string.Format("{0}/ExcelDBUpload/x86/ExcelDBUploadTool.exe", GetParentFullName); } }

    /// <summary> 엑셀 테이블 파일들이 있는 경로. </summary>
    public static string GetExcelPath { get { return string.Format("{0}/Resource/Excel", GetParentFullName); } }
    /// <summary> Export될 Bytes 파일들이 저장될 경로. </summary>
    public static string GetBytePath { get { return string.Format("{0}/Bundles/Bytes", Application.dataPath); } }
    /// <summary> Export될 Class 파일들이 저장될 경로. </summary>
    public static string GetCSPath { get { return string.Format("{0}/Scripts/Table", Application.dataPath); } }
}
