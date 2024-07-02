using System;

namespace IdealAPI.Repository
{
    public interface ICachingInterface
    {
        T GetData<T>(string key);

        bool SetData<T>(string key, T Value, TimeSpan? expirationTime);
        Object RemoveData(string key);
    }
}
