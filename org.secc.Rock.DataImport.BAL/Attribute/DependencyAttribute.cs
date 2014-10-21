using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Attribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public class DependencyAttribute : System.Attribute
    {
        public string Name { get; set; }
        public Type Dependency { get; set; }
        public int Order { get; set; }

        public DependencyAttribute( string name, Type dependency, int order = 0 )
        {
            Name = name;
            Dependency = dependency;
            Order = order;
        }
    }
}
