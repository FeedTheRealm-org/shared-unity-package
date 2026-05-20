namespace API
{
    [System.Serializable]
    public class DataEnvelope<T>
    {
        public T data;
    }

    [System.Serializable]
    public class ApiResponse<T>
    {
        public T data;
        public ErrorResponse error;
        public int status;

        public bool IsSuccess => status >= 200 && status < 300 && data != null;
        public string ErrorMessage => error?.detail ?? error?.title;
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
