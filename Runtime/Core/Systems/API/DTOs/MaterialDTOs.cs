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

        public override string ToString()
        {
            return $"MaterialResponse(id={id}, name={name}, world_id={world_id}, url={url}, created_at={created_at}, updated_at={updated_at})";
        }
    }

    [Serializable]
    public class MaterialResponseList
    {
        public MaterialResponse[] data;
    }
}
