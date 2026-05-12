using System;
using System.Collections.Generic;

namespace API
{
    [Serializable]
    public class CharacterColorHsv
    {
        public float h;
        public float s;
        public float v;
    }

    /* --- Requests --- */
    [Serializable]
    public class PatchCharacterInfoRequest
    {
        public string character_name;
        public string character_bio;
        public CharacterColorHsv skin_color;
        public CharacterColorHsv hair_color;
        public CharacterColorHsv eye_color;
        public Dictionary<string, string> category_sprites;
    }

    [Serializable]
    public class IssueWorldJoinTokenRequest
    {
        public string world_id;
    }

    [Serializable]
    public class ConsumeWorldJoinTokenRequest
    {
        public string token_id;
    }

    /* --- Responses --- */
    [Serializable]
    public class CharacterInfoResponse
    {
        public string character_name;
        public string character_bio;
        public CharacterColorHsv skin_color;
        public CharacterColorHsv hair_color;
        public CharacterColorHsv eye_color;
        public Dictionary<string, string> category_sprites;
    }

    [Serializable]
    public class WorldJoinTokenResponse
    {
        public string token_id;
        public string expires_at;
    }

    [Serializable]
    public class ConsumeWorldJoinTokenResponse
    {
        public string user_id;
        public string world_id;
    }
}
