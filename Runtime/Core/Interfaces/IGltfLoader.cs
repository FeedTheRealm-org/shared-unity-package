using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FTRShared.Runtime.Core.Interfaces
{
    public interface IGltfLoader
    {
        UniTask<GameObject> LoadModel(byte[] data);
    }
}
