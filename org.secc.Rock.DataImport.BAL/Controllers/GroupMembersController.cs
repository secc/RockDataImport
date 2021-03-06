﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class GroupMembersController :BaseController<GroupMember>
    {
        string BaseAPIPath = "/api/GroupMembers/";

        public GroupMembersController() : base() { }

        public GroupMembersController( RockService service ) : base( service ) { }

        public override void Add( GroupMember entity )
        {
            Service.PostData<GroupMember>( BaseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", id );
            Service.DeleteData( apiPath );
        }

        public override GroupMember GetById( int id )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", id );
            return Service.GetData<GroupMember>( apiPath );
        }

        public override GroupMember GetByGuid( Guid guid )
        {
            return GetByGuid( guid, false );
        }

        public GroupMember GetByGuid( Guid guid, bool includeDeceased = false )
        {
            Dictionary<string, string> queryString = new Dictionary<string, string>();


            if ( includeDeceased )
            {
                queryString = new Dictionary<string, string>();
                queryString.Add( "IncludeDeceased", Boolean.TrueString );
            }

            return Service.GetDataByGuid<GroupMember>( BaseAPIPath, guid, queryString );
        }

        public override List<GroupMember> GetAll()
        {
            return Service.GetData<List<GroupMember>>( BaseAPIPath );
        }

        public override List<GroupMember> GetByFilter( string expression )
        {
            return GetByFilter( expression, false );
        }

        public List<GroupMember> GetByFilter( string expression, bool includeDeceased = false )
        {
            Dictionary<string, string> queryString = null;

            if ( includeDeceased )
            {
                queryString = new Dictionary<string, string>();
                queryString.Add( "IncludeDeceased", Boolean.TrueString );
            }

            return Service.GetData<List<GroupMember>>( BaseAPIPath, expression, queryString );

        }

        public override GroupMember GetByForeignId( string foreignId )
        {
            return GetByForeignId( foreignId, false );
        }

        public GroupMember GetByForeignId( string foreignId, bool includeDeceased )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );

            return GetByFilter( expression, includeDeceased ).FirstOrDefault();
        }

        public override void Update( GroupMember entity )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", entity.Id );
            Service.PutData<GroupMember>( apiPath, entity );
        }

        public List<GroupMember> GetByGroupId( int groupId )
        {
            string expression = string.Format( "GroupId eq {0}", groupId );
            return GetByFilter( expression );
        }

        public List<GroupMember> GetByGroupIdPersonId( int groupId, int personId, bool includeDeceased = false )
        {
            string expression = string.Format( "GroupId eq {0} and PersonId eq {1}", groupId, personId );

            return GetByFilter( expression, includeDeceased );
        }
    }
}
