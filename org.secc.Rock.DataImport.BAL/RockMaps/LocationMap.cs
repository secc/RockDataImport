using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;
using SystemGuid = Rock.SystemGuid;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class LocationMap : MapBase
    {
        public RockService Service { get; set; }

        private LocationMap() { }

        public LocationMap(RockService service)
        {
            Service = service;
        }


        public int? SaveAddress( string street1, string city, string state, string country, string postalCode, string street2, double? latitude = null, double? longitude = null, string foreignKey = null, 
            string name = null, bool isActive = false, int? parentLocationId = null, int? locationId = null, int? locationTypeValueId = null )
        {
            Location location = null;
            if ( locationId != null )
            {
                location = GetLocationById( (int)locationId );

                if ( location == null )
                {
                    return null;
                }
            }
            else
            {
                location = GetLocationByAddress( street1, street2, city, state, postalCode );

                if ( location != null )
                {
                    return location.Id;
                }
                location = new Location();
            }

            location.ParentLocationId = parentLocationId;
            location.Name = name;
            location.IsActive = isActive;
            location.LocationTypeValueId = locationTypeValueId;

            //if ( latitude != null && longitude != null )
            //{
            //    location.SetLocationPointFromLatLong( (double) latitude, (double) longitude );
            //}

            location.Street1 = street1;
            location.Street2 = street2;
            location.City = city;
            location.State = state;
            location.Country = country;
            location.PostalCode = postalCode;
            location.ForeignId = foreignKey;

            return SaveLocation( location );

        }

        public int GetCampusLocationDefinedValueId()
        {
            DefinedValueMap dvMap = new DefinedValueMap( Service );
            Guid campusLocationGuid = new Guid( SystemGuid.DefinedValue.LOCATION_TYPE_CAMPUS );

            return dvMap.GetDefinedValueByGuid( campusLocationGuid ).Id;
        }

        public Dictionary<string, object> GetByForeignId( string foreignId )
        {
            LocationController controller = new LocationController( Service );
            Location l = controller.GetByForeignId( foreignId );

            return ToDictionary( l );
        }


        private Location GetLocationByAddress(string street1, string street2, string city, string state, string postalCode)
        {
            StringBuilder filterBuilder = new StringBuilder();

            filterBuilder.AppendFormat( "Street1 eq '{0}'", street1 );

            if ( String.IsNullOrWhiteSpace( street2 ) )
            {
                filterBuilder.Append( " and ( Street2 eq null or Street2 eq '')" );

            }
            else
            {
                filterBuilder.AppendFormat( " and Street2 eq '{0}'", street2 );
            }

            filterBuilder.AppendFormat( " and City eq '{0}'", city );
            filterBuilder.AppendFormat( " and State eq '{0}'", state );
            filterBuilder.AppendFormat( " and PostalCode eq '{0}'", postalCode );

            LocationController controller = new LocationController( Service );
            return controller.GetByFilter( filterBuilder.ToString() ).FirstOrDefault();

        }

        private Location GetLocationById( int locationId )
        {
            LocationController controller = new LocationController( Service );

            return controller.GetById( locationId );
        }

        private int? SaveLocation( Location l )
        {
            LocationController controller = new LocationController( Service );
            if ( l.Id == 0 )
            {
                l.CreatedByPersonAliasId = Service.LoggedInPerson.PrimaryAliasId;
                controller.Add( l );
            }
            else
            {
                l.ModifiedByPersonAliasId = Service.LoggedInPerson.PrimaryAliasId;
                controller.Update( l );
            }

            return controller.GetByGuid( l.Guid ).Id;
        }
    }
}
