﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Integration
{
    public class ExportMapEventArgs : EventArgs
    {

        public Type MapType { get; set; }
        public string Identifier { get; set; }
        public int? RockIdentifier { get; set; }
        public bool IsSuccess { get; set; }
    }
}
