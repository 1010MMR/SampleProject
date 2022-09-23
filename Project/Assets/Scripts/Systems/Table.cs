using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using TableData;

public class Table
{
    public int ID { get; protected set; }

    protected void Init<T>(RowData data, T obj) where T : Table
    {
        List<string> keyList = data.ColumnList;
        for (int i = 0; i < keyList.Count; i++)
        {
            PropertyInfo pInfo = typeof(T).GetProperty(keyList[i]);
            if (pInfo != null)
                pInfo.SetValue(obj, data.GetRowValue(keyList[i]).GetValue);
        }
    }
}
