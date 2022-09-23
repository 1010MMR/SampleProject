using TableData;

public enum ESOUNDEFFECT
{
	NONE = 0,
	SE_ATTACK_SHIELD_BREAK_01 = 1,
	SE_BUILD_PROCESS_01 = 2,
	SE_ROULETTE_STOP_01 = 3,
	SE_COMMON_UI_SHOW_01 = 4,
	SE_BUILD_PURCHASE_01 = 5,
	SE_COMMON_UI_HIDE_01 = 6,
	SE_ROULETTE_RESULT_SPECIAL_01 = 7,
	SE_ROULETTE_RESULT_SPECIAL_02 = 8,
	SE_ROULETTE_RESULT_SPECIAL_03 = 9,
}

public class SoundEffectTable : Table
{
	public string Name { get; set; }

	public SoundEffectTable(RowData data)
	{
		Init(data, this);
	}
}