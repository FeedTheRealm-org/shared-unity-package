namespace API
{
    [System.Serializable]
    public class DataEnvelope<T>
    {
        public T data;
    }

    [System.Serializable]
    public class ErrorResponse
    {
        public string type;
        public string title;
        public int status;
        public string detail;
        public string instance;
    }
}
