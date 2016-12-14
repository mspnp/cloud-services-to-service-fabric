namespace Tailspin.Web.Survey.Shared.Helpers
{
    using System;
    using StackExchange.Redis;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    internal static class TenantCacheHelper
    {
        private static readonly Lazy<ConnectionMultiplexer> lazyConnection =
            new Lazy<ConnectionMultiplexer>(
                () =>
                    ConnectionMultiplexer.Connect(
                        ServiceFabricConfiguration.GetConfigurationSettingValue("ConnectionStrings",
                            "RedisCacheConnectionString", string.Empty)));

        private static ConnectionMultiplexer Connection => lazyConnection.Value;

        private static string GetTenantKey(string tenant, string key)
        {
            return $"{tenant.ToLowerInvariant()}::{key.ToLowerInvariant()}";
        }

        internal static async Task AddToCacheAsync<T>(string tenant, string key, T @object) where T : class
        {
            try
            {
                IDatabase cache = Connection.GetDatabase();
                await cache.StringSetAsync(GetTenantKey(tenant, key), JsonConvert.SerializeObject(@object));
            }
            catch (TimeoutException e)
            {
                TraceHelper.TraceError(e.TraceInformation());
            }
            catch (RedisConnectionException e)
            {
                TraceHelper.TraceError(e.TraceInformation());
            }
        }

        internal static async Task<T> GetFromCacheAsync<T>(string tenant, string key, Func<Task<T>> @default) where T : class
        {
            var result = default(T);
            var success = false;

            IDatabase cache = Connection.GetDatabase();

            try
            {
                var cachedValue = await cache.StringGetAsync(GetTenantKey(tenant, key)).ConfigureAwait(false);
                if (!cachedValue.IsNull)
                {
                    result = JsonConvert.DeserializeObject<T>(cachedValue);
                    success = true;
                }
            }
            catch (TimeoutException e)
            {
                TraceHelper.TraceError(e.TraceInformation());
            }
            catch (RedisConnectionException e)
            {
                TraceHelper.TraceError(e.TraceInformation());
            }

            if (success == false && @default != null)
            {
                result = await @default().ConfigureAwait(false);
                if (result != null)
                {
                    await AddToCacheAsync(tenant.ToLowerInvariant(), key.ToLowerInvariant(), result).ConfigureAwait(false);
                }
            }
            TraceHelper.TraceInformation("cache {2} for {0} [{1}]", key, tenant, success ? "hit" : "miss");



            return result;
        }

        internal static Task RemoveFromCacheAsync(string tenant, string key)
        {
            IDatabase cache = Connection.GetDatabase();
            return cache.KeyDeleteAsync(GetTenantKey(tenant, key));
        }
    }
}
