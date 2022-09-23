using TableData;

public enum EETCVALUE
{
	NONE = 1,
}

public class ETCValueTable : Table
{
	public int Value { get; set; }

	public ETCValueTable(RowData data)
	{
		Init(data, this);
	}
}