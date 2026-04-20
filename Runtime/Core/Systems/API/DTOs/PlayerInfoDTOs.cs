using System;
using System.Collections.Generic;

namespace API
{
    /* --- Requests --- */
    [Serializable]
    public class PatchCharacterInfoRequest
    {
        public string character_name;
        public string character_bio;
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
