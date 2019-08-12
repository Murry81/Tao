using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoDatabaseService.Interfaces;

namespace TaoDatabaseService
{
    public class DataServiceConfiguration : IDataServiceConfiguration
    {
        public string DataBaseName => ConfigurationManager.AppSettings.Get("ServiceDatabaseName");
    }
}
