public struct PDAccesor
{
    public readonly string key;
    public readonly SimpleJSON.JSONObject json;

    public PDAccesor(string key, SimpleJSON.JSONObject json)
    {
        this.key = key;
        this.json = json;
    }
}

public interface IPersistentData
{
    void SaveData(PDAccesor accesor);
    void LoadData(PDAccesor accesor);
    void ClearData(string path);
    string DataPath();
}
