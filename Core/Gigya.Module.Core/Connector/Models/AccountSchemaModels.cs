using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.Core.Connector.Models
{
    public class AccountSchemaModel
    {
        public List<AccountSchemaProperty> Properties { get; set; }
    }

    public class AccountSchemaProperty
    {
        public string Name { get; set; }
    }
}
