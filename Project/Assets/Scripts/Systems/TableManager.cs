using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using ExcelBytesReader;

/// <summary>
/// 테이블을 관리할 클래스. Init 시 모든 테이블을 Parse합니다.
/// </summary>
public class TableManager : MonoSingleton<TableManager>
{
    #region VALUES

    private Dictionary<TableEnums, Dictionary<int, Table>> Table = null;

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
        Table = new Dictionary<TableEnums, Dictionary<int, Table>>();
        List<TableData.RowData> dataList = null;

        for (TableEnums enums = 0; enums < TableEnums.MAX; enums++)
        {
            Table.Add(enums, new Dictionary<int, Table>());

            string enumStr = enums.ToString();
            TextAsset asset = ResourceLoader.LoadData(enumStr);

            BytesReader reader = new BytesReader(asset.bytes);

            if (string.IsNullOrEmpty(reader.ErrorLog) && reader.GetColumnsTable(out dataList))
            {
                Type type = Type.GetType(string.Format("{0}Table", enumStr));

                for (int count = 0; count < dataList.Count; count++)
                {
                    Table result = (Table)Activator.CreateInstance(type, args: new object[] { dataList[count] });
                    if (result != null)
                    {
                        if (Table[enums].ContainsKey(result.ID) == false) Table[enums].Add(result.ID, result);
#if ENABLE_LOG
                        else Debug.Log(string.Format("[ TableManager ][ Init ] : {0} in same key. {1}", enums, result.ID));
#endif
                    }
                }
            }
        }
    }

    protected override void Release()
    {
        Table = null;
    }

    protected override void Clear()
    {
        Release();
        base.Clear();
    }

    #endregion

    #region PUBLIC METHOD

    /// <summary>
    /// TableEnums에 해당하는 테이블을 반환합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public Dictionary<int, T> GetTable<T>(TableEnums type) where T : Table
    {
        Dictionary<int, T> table = new Dictionary<int, T>();
        if (Table.ContainsKey(type))
        {
            foreach (int key in Table[type].Keys)
                table.Add(key, (T)Table[type][key]);
        }

        return table;
    }

    /// <summary>
    /// TableEnums에 해당하는 테이블 리스트를 반환합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<T> GetTableList<T>(TableEnums type) where T : Table
    {
        List<T> list = new List<T>();
        if (Table.ContainsKey(type))
        {
            foreach (int key in Table[type].Keys)
                list.Add((T)Table[type][key]);
        }

        return list;
    }

    /// <summary>
    /// TableEnums에 해당하는 테이블 리스트 중 동일한 id를 가진 요소를 반환합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public T GetTableRow<T>(TableEnums type, int id) where T : Table
    {
        return (Table.ContainsKey(type) && Table[type].ContainsKey(id)) ? (T)Table[type][id] : default;
    }

    /// <summary>
    /// TableEnums에 해당하는 테이블 리스트의 숫자를 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetTableCount(TableEnums type)
    {
        return Table.ContainsKey(type) ? Table[type].Count : 0;
    }

    /// <summary>
    /// TableEnums에 해당하는 테이블 리스트의 첫 번째 인덱스의 ID를 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetTableFirstID(TableEnums type)
    {
        return Table.ContainsKey(type) ? Table[type].First().Key : 0;
    }

    /// <summary>
    /// TableEnums에 해당하는 테이블 리스트의 마지막 인덱스의 ID를 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetTableLastID(TableEnums type)
    {
        return Table.ContainsKey(type) ? Table[type].Last().Key : 0;
    }

    /// <summary>
    /// ETCValueTable 중 EETCVALUE에 해당하는 값을 반환합니다.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public float GetETCTableValue(EETCVALUE value)
    {
        return (Table[TableEnums.ETCValue][(int)value] as ETCValueTable).Value;
    }

    #endregion

    #region PRIVATE METHOD

    #endregion
}
