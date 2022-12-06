namespace Viettel.Domain.Interfaces.Services
{
    using DomainModel;
    using System;
    using System.Collections.Generic;

    public partial interface ICacheService
    {
        T Get<T>(string key);

        /// <summary>
        /// Cache objects for a specified amount of time
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="data">Object / Data to cache</param>
        /// <param name="minutesToCache">How many minutes to cache them for</param>
        void Set(string key, object data, CacheTimes minutesToCache);
        void Set(string key, object data, int secondToCache);


        bool IsSet(string key);
        void Invalidate(string key);
        void Clear();
        void ClearStartsWith(string keyStartsWith);
        void ClearStartsWith(List<string> keysStartsWith);
        T CachePerRequest<T>(string cacheKey, Func<T> getCacheItem);
        T CachePerRequest<T>(string cacheKey, Func<T> getCacheItem, CacheTimes minutesToCache);
        T CachePerRequest<T>(string cacheKey, Func<T> getCacheItem, int secondToCache);
        T CachePerRequest<T>(string cacheKey, Func<T> getCacheItem, CacheTimes minutesToCache, bool cacheInDebug);
        void SetPerRequest(string cacheKey, object objectToCache);
    }
}
