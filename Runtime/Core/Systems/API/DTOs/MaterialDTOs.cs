using System;
using System.Collections.Generic;

namespace API
{
    [Serializable]
    public class MaterialResponse
    {
        public string id;
        public string name;
        public string world_id;
        public string url;
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class MaterialResponseList
    {
        public MaterialResponse[] data;
    }
}
