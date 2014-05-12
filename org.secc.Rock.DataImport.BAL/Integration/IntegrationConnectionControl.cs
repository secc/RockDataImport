using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Integration
{
    public abstract class IntegrationConnectionControl : System.Windows.Controls.UserControl
    {
        public abstract Dictionary<string, string> Value { get; }

        public IntegrationConnectionControl() : base ()
        {
        }


        public abstract void Load();

        public abstract void Load(Dictionary<string,string> settings);

        public abstract void Clear();

        public abstract bool IsValid( ref Dictionary<string, string> errors );




    }
}
