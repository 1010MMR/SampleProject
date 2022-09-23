using TableData;

public enum EEFFECT
{
	VFX_SPARKLEAREAYELLOW = 1,
	VFX_SLOTMONEYFOUNTAINLV1 = 2,
	VFX_SLOTMONEYFOUNTAINLV2 = 3,
	VFX_SLOTMONEYFOUNTAINLV3 = 4,
	VFX_SPINSPARK = 5,
	VFX_SHIELD = 6,
	VFX_DEBRIS = 7,
	VFX_ATTACK = 8,
}

public class EffectTable : Table
{
	public string Name { get; set; }
	public float EffectTime { get; set; }
	public EFFECT_DUMMY_TYPE DummyType { get; set; }
	public string EffectLayerName { get; set; }

	public EffectTable(RowData data)
	{
		Init(data, this);
	}
}