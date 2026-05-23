using Newtonsoft.Json;

public enum StatusTickTiming
{
    TurnStart,
    TurnEnd
}

[System.Serializable]
public class StatusEffectData : IHasID
{
    public string id;
    string IHasID.id => id;

    public string displayName;

    public int defaultDuration = 1;
    public int maxStacks = 1;

    public int tickDamage = 0;
    public int attackModifier = 0;
    public int defenseModifier = 0;

    public bool disableMove = false;

    [JsonProperty("tickTiming")]
    public StatusTickTiming tickTiming = StatusTickTiming.TurnEnd;
}