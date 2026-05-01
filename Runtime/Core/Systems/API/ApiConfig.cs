using UnityEngine;

public enum ApiEnvironment
{
    Local,
    Prod,
}

[CreateAssetMenu(fileName = "ApiConfig", menuName = "Scriptable Objects/API/ApiConfig")]
public class ApiConfig : ScriptableObject
{
    [SerializeField]
    private ApiEnvironment _environment = ApiEnvironment.Local;

    [Header("Local")]
    [SerializeField]
    private string _localHostname = "http://localhost";

    [SerializeField]
    private int _localPort = 8000;

    [SerializeField]
    private string _localModelsCDN = "http://localhost:8001/worlds";

    [SerializeField]
    private string _localCosmeticsCDN = "http://localhost:8001/cosmetics";

    [Header("Prod")]
    [SerializeField]
    private string _prodHostname = "https://core.feedtherealm.world";

    [SerializeField]
    private int _prodPort = 443;

    [SerializeField]
    private string _prodModelsCDN = "https://d632z6itue3st.cloudfront.net/";

    [SerializeField]
    private string _prodCosmeticsCDN = "https://d3ry8oaxnx8r71.cloudfront.net/";

    public string Hostname => _environment == ApiEnvironment.Local ? _localHostname : _prodHostname;
    public int Port => _environment == ApiEnvironment.Local ? _localPort : _prodPort;
    public string ModelsCDN =>
        _environment == ApiEnvironment.Local ? _localModelsCDN : _prodModelsCDN;
    public string CosmeticsCDN =>
        _environment == ApiEnvironment.Local ? _localCosmeticsCDN : _prodCosmeticsCDN;
}
