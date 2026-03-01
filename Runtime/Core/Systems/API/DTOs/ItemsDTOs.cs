namespace API
{
    [System.Serializable]
    public class UploadItemSpritesRequest
    {
        public string[] ids;
        public string[] sprites;
    }

    [System.Serializable]
    public class AddItemCategoryRequest
    {
        public string category_name;
    }

    [System.Serializable]
    public class ItemCategoryResponse
    {
        public string category_id;
        public string category_name;
    }

    [System.Serializable]
    public class ItemResponse
    {
        public string id;
        public string url;
        public string created_at;
        public string updated_at;
    }

    [System.Serializable]
    public class ItemListResponse
    {
        public ItemResponse[] items;
    }

    [System.Serializable]
    public class ItemCategoryListResponse
    {
        public ItemCategoryResponse[] category_list;
    }
}
