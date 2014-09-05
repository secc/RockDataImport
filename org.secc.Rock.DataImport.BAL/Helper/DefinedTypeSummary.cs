using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Helper
{
    public class DefinedTypeSummary
    {
        private int mOrder = 0;
        private bool mIsSystem = false;
        
        public string Id { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ForeignId { get; set; }
        public Guid UniqueIdentifier { get; set; }

        public int Order
        {
            get
            {
                return mOrder;
            }
            set
            {
                mOrder = value;
            }
        }

        public bool IsSystem
        {
            get
            {
                return mIsSystem;
            }
            set
            {
                mIsSystem = value;
            }

        }

        public List<DefinedValueSummary> ValueSummaries { get; set; }

        public DefinedTypeSummary() { }

        public DefinedTypeSummary( DefinedType dt )
        {
            if ( dt != null )
            {
                Id = dt.Id.ToString();
                Category = dt.Category;
                Name = dt.Name;
                Description = dt.Description;
                UniqueIdentifier = dt.Guid;
                ForeignId = dt.ForeignId;
                IsSystem = dt.IsSystem;

                if(dt.DefinedValues != null && dt.DefinedValues.Count > 0)
                {
                    ValueSummaries = dt.DefinedValues.Select( dv => new DefinedValueSummary()
                                        {
                                            Id = dv.Id.ToString(),
                                            DefinedTypeId = dv.DefinedTypeId.ToString(),
                                            Value = dv.Value,
                                            Description = dv.Description,
                                            ForeignId = dv.ForeignId,
                                            Order = dv.Order,
                                            IsSystem = dv.IsSystem
                                        } ).ToList();

                }

            }
        }
    }
}
