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