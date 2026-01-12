using UnityEngine;

[CreateAssetMenu(fileName = "ApiConfig", menuName = "Scriptable Objects/API/ApiConfig")]
public class ApiConfig : ScriptableObject
{
    public string Hostname;
    public int Port;
}
