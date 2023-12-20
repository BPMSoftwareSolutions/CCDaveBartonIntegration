using DaveBartonCCIntegrationAPI.DataAccess;

namespace DaveBartonCCIntegrationAPI
{
    public class ServiceScope
    {
        public SqlServerRepository Repository { get; set; }
        public string ClientSecret { get; set; }
        public string AESKey { get; set; }

        public ServiceScope()
        {
        }
    }
}
