using TableData;

public enum ESOUNDBGM
{
	NONE = 0,
	BGM_TITLE = 1,
	BGM_ROULETTE = 2,
	BGM_ATTACK = 3,
	BGM_TOWN = 4,
	BGM_CHAPTER_COMPLETE = 5,
	BGM_CHAPTER_CLEAR = 6,
}

public class SoundBGMTable : Table
{
	public string Name { get; set; }

	public SoundBGMTable(RowData data)
	{
		Init(data, this);
	}
}