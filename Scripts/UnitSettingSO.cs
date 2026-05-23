using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class UnitSpawnSetting : IHasID
{
    public string id;
    string IHasID.id => id;
    [JsonProperty("name")] public string displayName;
    [JsonProperty("faction")] public Clan clan;
    public int hp;
    [JsonProperty("atk")] public int attack;
    [JsonProperty("def")] public int defense;
    [JsonProperty("unit_type")] public PieceType type;
    public string passive_id;
    public string on_hit_status_id;
    public string sprite_id;
}

[CreateAssetMenu(menuName = "Setting/Others")]
public class UnitSettingSO : ScriptableObject
{
    public UnitSpawnSetting data;
}

public interface IUnitSetting
{
    string displayName { get; set; }
    Clan clan { get; set; }
    int hp { get; set; }
    int attack { get; set; }
    int defense { get; set; }
    PieceType type { get; set; }
    string passive_id { get; set; }
    string sprite_id { get; set; }
}