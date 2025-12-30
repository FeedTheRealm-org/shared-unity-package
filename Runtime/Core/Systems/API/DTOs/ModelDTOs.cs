using System.Collections.Generic;

namespace API
{
    [System.Serializable]
    public class AssetListResponse
    {
        public AssetListData data;
    }

    [System.Serializable]
    public class AssetListData
    {
        public string world_id;
        public List<AssetListItem> models;
    }

    [System.Serializable]
    public class AssetListItem
    {
        public string model_id;
        public string name;
    }
}
