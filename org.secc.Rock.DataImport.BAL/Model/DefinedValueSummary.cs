using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.secc.Rock.DataImport.BAL.Model
{
    public class DefinedValueSummary
    {
        public string Id { get; set; }
        public string DefinedTypeId { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }
}
