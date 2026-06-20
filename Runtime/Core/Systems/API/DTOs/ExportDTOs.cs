using System;
using System.Collections.Generic;

namespace API
{
    [Serializable]
    public class ExportEntry
    {
        public string app_name;
        public string version;
        public string os;
        public string path;
        public string release_note;
        public bool is_latest;
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class ExportsListResponse
    {
        public List<ExportEntry> data;
    }
}
