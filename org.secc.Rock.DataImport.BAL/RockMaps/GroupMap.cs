using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;



namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class GroupMap 
    {
        RockService Service { get; set; }

        private GroupMap() { }

        public GroupMap( RockService service )
        {
            Service = service;
        }

    }
}
