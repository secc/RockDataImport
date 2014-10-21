using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Helper
{
    public class DefinedValueSummary
    {
        private List<string> mForeignIdValues;
        private int mOrder = 0;
        private bool mIsSystem = false;

        public string Id { get; set; }
        public string DefinedTypeId { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }


        public string ForeignId
        {
            get
            {
                if ( ForeignIdValues.Count == 0 )
                {
                    return null;
                }
                else
                {
                    return string.Join( "|", ForeignIdValues );
                }

            }
            set
            {
                if ( !String.IsNullOrWhiteSpace( value ) )
                {
                    ForeignIdValues = value.Split( new char[] { '|' } ).ToList();
                }
                else
                {
                    ForeignIdValues.Clear();
                }
                
            }
        }

        public List<string> ForeignIdValues
        {
            get
            {
                return mForeignIdValues;
            }
            set
            {
                mForeignIdValues = value;
            }
        }

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
                mIsSystem = false;
            }
        }

        public DefinedValueSummary() 
        {
            mForeignIdValues = new List<string>();
        }

        public DefinedValueSummary( DefinedValue dv ) 
        {
            mForeignIdValues = new List<string>();
            Id = dv.Id.ToString();
            DefinedTypeId = dv.DefinedTypeId.ToString();
            Value = dv.Value;
            Description = dv.Description;
            ForeignId = dv.ForeignId;
            Order = dv.Order;
            IsSystem = dv.IsSystem;
        }

    }
}
