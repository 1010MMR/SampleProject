using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class ExcelDBUploader
{
    private const string DB_UPLOAD_TABLE_LIST = "ChapterInfo,ChapterMapTileInfo,ChapterReward,TutorialReward,SuperAttackScoreboard,SuperAttackResult,MansionInfo,MansionRoomInfo,MansionItemInfo,MansionMission,MansionRankReward,ShopInfo,ShopInfoDetail";

    [MenuItem("Tools/Server/Excel Upload (x64)", false, 1)]
    private static void ExcelToBytesMenu_x64()
    {
        Process process = new Process();
        ProcessStartInfo info = new ProcessStartInfo(ExcelToBytesProperty.GetExcelDBUploaderToolPath_x64, string.Format("{0},{1},{2}", ExcelToBytesProperty.GetExcelPath,
                                                                                                                                                                "DataDB",
                                                                                                                                                                DB_UPLOAD_TABLE_LIST));

        process.StartInfo = info;
        process.Start();
    }

    [MenuItem("Tools/Server/Excel Upload (x86)", false, 2)]
    private static void ExcelToBytesMenu_x86()
    {
        Process process = new Process();
        ProcessStartInfo info = new ProcessStartInfo(ExcelToBytesProperty.GetExcelDBUploaderToolPath_x86, string.Format("{0},{1},{2}", ExcelToBytesProperty.GetExcelPath,
                                                                                                                                                                "DataDB",
                                                                                                                                                                DB_UPLOAD_TABLE_LIST));

        process.StartInfo = info;
        process.Start();
    }
}
