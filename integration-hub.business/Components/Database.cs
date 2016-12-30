using PensionConsultants.Data.Access;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationHub.Business.Components
{
    public static class Database
    {
        private static DataAccessComponent.Connections _connection = DataAccessComponent.Connections.PCIDB_DataIntegrationHub;
        private static DataAccessComponent.SecurityTypes _securityType = DataAccessComponent.SecurityTypes.Impersonate;
        public static DataAccessComponent DataIntegrationHub = new DataAccessComponent(_connection, _securityType);

        public static bool Succeeded()
        {
            // Check connection and return bool
            return DataIntegrationHub.ConnectionSucceeded();
        }
    }
}
