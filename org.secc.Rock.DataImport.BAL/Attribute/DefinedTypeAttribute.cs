using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Attribute
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public class DefinedTypeAttribute : System.Attribute
    {
        public string Name { get; set; }
        public string RockDefinedTypeGuid { get; set; }
        public string SourceDefinedTypeIdentifier { get; set; }
        public int Order { get; set; }

        public DefinedTypeAttribute( string name, string rockDefinedTypeGuid, string sourceDefinedTypeIdentifier, int order = 0 )
        {
            Name = name;
            RockDefinedTypeGuid = rockDefinedTypeGuid;
            SourceDefinedTypeIdentifier = sourceDefinedTypeIdentifier;
            Order = order;
        }
    }
}
