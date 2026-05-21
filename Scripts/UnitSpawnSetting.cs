using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Setting/Others")]
public class UnitSpawnSetting : ScriptableObject, IHasID
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
    public string sprite_id;
}