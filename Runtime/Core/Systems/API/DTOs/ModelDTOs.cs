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

    [System.Serializable]
    public class ModelInfo
    {
        public string model_id;
        public string url;
    }

    [System.Serializable]
    public class WorldModelsData
    {
        public string world_id;
        public ModelInfo[] models;
    }

    [System.Serializable]
    public class WorldModelsResponse
    {
        public WorldModelsData data;
    }
}
