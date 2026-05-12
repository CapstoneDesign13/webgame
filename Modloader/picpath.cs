[System.Serializable]
public class PicPath : IHasID
{
    public string id;
    string IHasID.id => id;
    public string path;
}