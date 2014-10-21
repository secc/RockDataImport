using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;

using Rock.Model;
using Rock.Net;

namespace org.secc.Rock.DataImport.BAL.Integration
{
    [Export(typeof(IIntegrationComponent))]
    [ExportMetadata("Name", "RockRMS")]
    [ExportMetadata("Description", "Rock RMS tester")]
    public class RockIntegration : IIntegrationComponent, global::Rock.Attribute.IHasAttributes
    {
        public const string IDENTIFIER = "RockRMS";

        public bool TestConnection( out string errorMessage )
        {
            bool isSuccess = false;
            errorMessage = null;

            string URL = "http://localhost:6229";
            string Username = "admin";
            string Password = "admin";

            try
            {
                var client = new RockRestClient( URL );
                client.Login( Username, Password );

                Person p = client.GetData<Person>( string.Format( "api/People/GetByUserName/{0}", Username ) );
                p.Aliases = client.GetData<List<PersonAlias>>( "api/PersonAlias/", "PersonId eq " + p.Id );
                isSuccess = true;

            }
            catch ( Exception ex )
            {

                errorMessage = string.Format( "ERROR {0} ", ex.Message );
            }

            return isSuccess;

        }

        [ImportMany(RockIntegration.IDENTIFIER, typeof(iExportMapComponent))]
        public List<Lazy<iExportMapComponent, iExportMapData>> ExportMaps
        {
            get;
            set;
        }

        public int Id
        {
            get { throw new NotImplementedException(); }
        }

        public Dictionary<string, global::Rock.Web.Cache.AttributeCache> Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<string, List<AttributeValue>> AttributeValues
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<string, string> AttributeValueDefaults
        {
            get { throw new NotImplementedException(); }
        }

        public string GetAttributeValue( string key )
        {
            throw new NotImplementedException();
        }

        public List<string> GetAttributeValues( string key )
        {
            throw new NotImplementedException();
        }

        public void SetAttributeValue( string key, string value )
        {
            throw new NotImplementedException();
        }
    }

    [Export(RockIntegration.IDENTIFIER, typeof(iExportMapComponent))]
    [ExportMetadata("Name", "RockTest")]
    public class RockTestMap : iExportMapComponent
    {
        public int RecordCount
        {
            get { throw new NotImplementedException(); }
        }

        public List<string> GetSubsetIDs( int startingRecord, int size )
        {
            throw new NotImplementedException();
        }

        public void ExportRecord( string identifier )
        {
            throw new NotImplementedException();
        }

        public event EventHandler<EventArgs> OnExportSuccess;

        public event EventHandler<EventArgs> OnExportFailure;
    }

}
