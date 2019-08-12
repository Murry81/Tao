using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoDatabaseService
{
    public partial class CustomerNameTaoEntities
    {
        public CustomerNameTaoEntities(string databaseName)
           : base(ConfigurationManager.ConnectionStrings["CustomerNameTaoEntities"].ConnectionString.Replace("databasename", databaseName))
        {
        }
    }
}
