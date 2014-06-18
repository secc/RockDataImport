using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Model
{
    public class DefinedTypeSummary
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid UniqueIdentifier { get; set; }
        public List<DefinedValueSummary> ValueSummaries { get; set; }

        public DefinedTypeSummary() { }

        public DefinedTypeSummary( DefinedType dt )
        {
            if ( dt != null )
            {
                Id = dt.Id.ToString();
                Name = dt.Name;
                Description = dt.Description;
                UniqueIdentifier = dt.Guid;

                if(dt.DefinedValues != null && dt.DefinedValues.Count > 0)
                {
                    ValueSummaries = dt.DefinedValues.Select( dv => new DefinedValueSummary()
                                        {
                                            Id = dv.Id.ToString(),
                                            DefinedTypeId = dv.DefinedTypeId.ToString(),
                                            Value = dv.Name,
                                            Order = dv.Order,

                                        } ).ToList();

                }

            }
        }
    }
}
