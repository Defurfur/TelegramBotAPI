using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text;
using XSystem.Security.Cryptography;

namespace ScheduleUpdateService.Services
{
    public interface IHashingService
    {
        public string GetHashSum<T>(T tObject);
    }
    public class HashingService : IHashingService
    {
        public string _hashingData = string.Empty;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly MD5CryptoServiceProvider _MD5ServiceProvider;
        public HashingService()
        {
            _serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new IgnorePropertiesResolver(new[] { "Id, Hash" })
            };

            _MD5ServiceProvider = new MD5CryptoServiceProvider();
        }
        private void GetHashingStringByJSON<T>(T tObject)
        {
            _hashingData = JsonConvert.SerializeObject(tObject, Formatting.None, _serializerSettings);
        }

        public string GetHashSum<T>(T tObject)
        {
            GetHashingStringByJSON(tObject);
            byte[] dataByteArray = Encoding.UTF8.GetBytes(_hashingData);
            byte[] byteHash = _MD5ServiceProvider.ComputeHash(dataByteArray);
            string hash = Convert.ToHexString(byteHash);

            return hash;
        }

    }

    public class IgnorePropertiesResolver : DefaultContractResolver
    {
        private readonly HashSet<string> _ignoreProps;
        public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
        {
            _ignoreProps = new HashSet<string>(propNamesToIgnore);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (_ignoreProps.Contains(property.PropertyName))
            {
                property.ShouldSerialize = _ => false;
            }
            return property;
        }
    }

}
