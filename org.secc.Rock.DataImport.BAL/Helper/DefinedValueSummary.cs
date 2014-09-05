using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.secc.Rock.DataImport.BAL.Helper
{
    public class DefinedValueSummary
    {
        private int mOrder = 0;
        private bool mIsSystem = false;

        public string Id { get; set; }
        public string DefinedTypeId { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string ForeignId { get; set; }

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

    }
}
