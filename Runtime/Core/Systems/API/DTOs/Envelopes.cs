using UnityEngine;
using System.Collections;

namespace API {
    [System.Serializable]
    class DataEnvelope<T> {
        public T data;
    }

    [System.Serializable]
    class ErrorResponse {
        public string type;
        public string title;
        public int status;
        public string detail;
        public string instance;
    }
}
