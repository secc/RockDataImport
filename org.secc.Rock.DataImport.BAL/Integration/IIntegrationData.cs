﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Integration
{
    public interface IIntegrationData
    {
        string Name { get; }
        string Description { get; }
    }
}
