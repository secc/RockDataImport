using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;


namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class GroupLocationMap : MapBase
    {
        RockService Service { get; set; }

        private GroupLocationMap() { }

        public GroupLocationMap( RockService service )
        {
            Service = service;
        }

        public int? SaveGroupLocation( int groupId, int locationId, int? groupLocationTypeValueId = null, int? groupMemberPersonAliasId = null, 
                bool isMailingLocation = false, bool isMappedLocation = false, string foreignId = null, int? groupLocationId = null )
        {
            GroupLocationController groupLocationController = new GroupLocationController( Service );
            GroupLocation groupLocation = null;

            if ( groupLocationId != null )
            {
                groupLocation = groupLocationController.GetById( (int)groupLocationId );

                if ( groupLocation == null )
                {
                    return null;
                }
            }
            else
            {
                groupLocation = new GroupLocation();
            }

            groupLocation.GroupId = groupId;
            groupLocation.LocationId = locationId;
            groupLocation.GroupLocationTypeValueId = groupLocationTypeValueId;
            groupLocation.IsMailingLocation = isMailingLocation;
            groupLocation.IsMappedLocation = isMappedLocation;
            groupLocation.GroupMemberPersonAliasId = groupMemberPersonAliasId;
            groupLocation.ForeignId = foreignId;

            int? personAliasId = Service.GetCurrentPersonAliasId();
            if ( groupLocationId == null )
            {
                groupLocation.CreatedByPersonAliasId = personAliasId;
                groupLocationController.Add( groupLocation );
            }
            else
            {
                groupLocation.ModifiedByPersonAliasId = personAliasId;
                groupLocationController.Update( groupLocation );
            }

            groupLocation = groupLocationController.GetByGuid( groupLocation.Guid );

            return groupLocation.Id;
        }

        public Dictionary<int, Dictionary<string, object>> GetGroupLocationByGroupIdLocationId( int groupID, int locationId )
        {
            GroupLocationController controller = new GroupLocationController( Service );

            string expression = string.Format("GroupId eq {0} and LocationId eq {1}", groupID, locationId);

            var groupLocations = controller.GetByFilter( expression );

            Dictionary<int, Dictionary<string, object>> groupLocationsDictionary = new Dictionary<int, Dictionary<string, object>>();

            foreach ( var gl in groupLocations )
            {
                groupLocationsDictionary.Add( gl.Id, ToDictionary( gl ) );
            }

            return groupLocationsDictionary;
        }
    }
}
