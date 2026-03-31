using System;
using System.Collections.Generic;

namespace API
{
    [Serializable]
    public class AssetListResponse
    {
        public AssetListData data;
    }

    [Serializable]
    public class AssetListData
    {
        public string world_id;
        public List<AssetListItem> models;
    }

    [Serializable]
    public class AssetListItem
    {
        public string model_id;
        public string name;
    }

    [Serializable]
    public class ModelInfo
    {
        public string model_id;
        public string url;
    }

    [Serializable]
    public class WorldModelsData
    {
        public string world_id;
        public List<ModelInfo> models = new();
    }

    [Serializable]
    public class WorldModelsResponse
    {
        public WorldModelsData data;
    }

    public class ModelRequest
    {
        public string id;
        public string filePath;
    }
}
