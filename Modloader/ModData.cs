using System.Collections.Generic;

[System.Serializable]
public class ModDataBase<T>
{
    public List<T> items;
}

public interface IHasID
{
    string id { get; }
}

public class Data_picpath : ModDataBase<PicPath> { }
public class Data_unitspawn : ModDataBase<UnitSpawnSetting> { }
public class Data_spawnpool : ModDataBase<SpawnPool> { }
public class Data_active : ModDataBase<Active> { }
public class Data_passive : ModDataBase<Passive> { }
public class Data_droptable : ModDataBase<DropTable> { }
public class Data_status_effect : ModDataBase<StatusEffectData> { }
public class Data_dialog : ModDataBase<PubDialog> { }
public class Data_oracle : ModDataBase<Oracle> { }