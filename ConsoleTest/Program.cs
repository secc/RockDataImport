using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Integration;

namespace ConsoleTest
{
    class Program
    {
        static void Main( string[] args )
        {
            Console.WriteLine( "Arena Data Import" );
            Console.WriteLine();

            IntegrationContainer container = new IntegrationContainer( System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Plugins" ) );

            var exports = container.GetIntegrations();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine( string.Format( "{0} integrations found.", exports.Count() ) );
            Console.ResetColor();

            if ( exports.Any() )
            {
                foreach ( var i in exports )
                {
                    Console.WriteLine( string.Format( "Integration - {0}", i.Name ) );

                    foreach ( var m in i.Component.ExportMaps )
                    {
                        Console.WriteLine( string.Format( "Export Map - {0}:{1}", i.Name, m.Key ) );

                       // Console.WriteLine( string.Format( "{0}: {1} record{2}.", m.Metadata.Name, m.Value.RecordCount, m.Value.RecordCount == 1 ? String.Empty : "s" ) );
                    }

                    
                }


                Console.ReadKey();

            }
        }

    }
}
