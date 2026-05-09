using System.IO;
using UnityEngine;

namespace Session
{
    public class SessionRepository
    {
        private readonly string _path = Path.Combine(
            Application.persistentDataPath,
            "session.json"
        );

        public void Save(SessionData data)
        {
            File.WriteAllText(_path, JsonUtility.ToJson(data));
        }

        public SessionData Load()
        {
            if (!File.Exists(_path))
                return null;
            return JsonUtility.FromJson<SessionData>(File.ReadAllText(_path));
        }

        public void Delete()
        {
            if (File.Exists(_path))
                File.Delete(_path);
        }
    }
}
