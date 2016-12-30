using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationHub.Business.Entities
{
    public struct Address
    {
        public string Line1 { get; internal set; }
        public string Line2 { get; internal set; }
        public string City { get; internal set; }
        public string State { get; internal set; }
        public string ZipCode { get; internal set; }
    }
}
