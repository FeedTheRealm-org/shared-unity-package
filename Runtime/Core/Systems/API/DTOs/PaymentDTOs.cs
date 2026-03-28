namespace API
{
    [System.Serializable]
    public class UpdateGemBalanceRequest
    {
        public string user_id;
        public int gems;
    }

    [System.Serializable]
    public class CheckoutRequest
    {
        public string gem_pack_id;
        public string success_url;
        public string cancel_url;
    }

    [System.Serializable]
    public class GemBalanceResponse
    {
        public string user_id;
        public int gems;
    }

    [System.Serializable]
    public class GemPackResponse
    {
        public string id;
        public string name;
        public int gems;
        public string price;
        public string created_at;
        public string updated_at;
    }

    [System.Serializable]
    public class CheckoutResponse
    {
        public string checkout_url;
    }
}
