using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using SystemGuid = Rock.SystemGuid;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class GroupMap : MapBase
    {
        RockService Service { get; set; }

        private GroupMap() { }

        public GroupMap( RockService service )
        {
            Service = service;
        }

        public Dictionary<string, object> GetFamilyGroupByForeignId( string foreignId )
        {
            int familyGroupTypeId = GetGroupTypeByGuid( new Guid( SystemGuid.GroupType.GROUPTYPE_FAMILY ) ).Id;
            GroupController controller = new GroupController( Service );
            Group family = controller.GetByForeignIdGroupType( foreignId, familyGroupTypeId );

            return ToDictionary( family );
        }

        public int GetFamilyAdultGroupRoleId()
        {
            Guid adultRoleGuid = new Guid( SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT );
            return GetGroupTypeRoleByGuid( adultRoleGuid ).Id;
        }

        public int GetFamilyChildGroupRoleId()
        {
            Guid childRoleGuid = new Guid( SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD );
            return GetGroupTypeRoleByGuid( childRoleGuid ).Id;
        }

        public Dictionary<string, object> GetGroupById( int groupId )
        {
            GroupController controller = new GroupController( Service );
            var group = controller.GetById( groupId );

            return ToDictionary( group );
        }

        public Dictionary<int, Dictionary<string, object>> GetGroupMemberByGroupIdPersonId( int groupId, int personId )
        {
            GroupMembersController groupMemberController = new GroupMembersController( Service );
            var groupMembers = groupMemberController.GetByGroupIdPersonId( groupId, personId );

            Dictionary<int, Dictionary<string, object>> groupMembersDictionary = new Dictionary<int, Dictionary<string, object>>();

            foreach ( var gm in groupMembers )
            {
                groupMembersDictionary.Add( gm.Id, ToDictionary( gm ) );
            }

            return groupMembersDictionary;

        }

        public int? SaveFamily(int? campusId, string familyName, string description = null,  string foreignId = null, int? familyId = null)
        {
            Guid familyGroupTypeGuid = new Guid( SystemGuid.GroupType.GROUPTYPE_FAMILY );
            int familyGroupTypeId = GetGroupTypeByGuid( familyGroupTypeGuid ).Id;

            return SaveGroup( familyGroupTypeId, familyName, foreignId: foreignId, groupId: familyId, campusId: campusId, description: description );

        }

        public int? SaveImpliedRelationshipsGroup( int ownerPersonId, int? groupId = null )
        {
            Guid impliedRelationshipTypeGuid = new Guid( SystemGuid.GroupType.GROUPTYPE_IMPLIED_RELATIONSHIPS );
            int impliedRelationshipTypeId = GetGroupTypeByGuid( impliedRelationshipTypeGuid ).Id;

            int? impliedGroupId = SaveGroup( impliedRelationshipTypeId, "Implied Relationships", groupId: groupId );

            if ( impliedGroupId != null )
            {
                GroupTypeRoleController roleController = new GroupTypeRoleController( Service );
                Guid ownerRoleGuid = new Guid( SystemGuid.GroupType.GROUPTYPE_IMPLIED_RELATIONSHIPS );
                int ownerRoleId = roleController.GetByGuid( ownerRoleGuid ).Id;

                SaveGroupMember( (int)impliedGroupId, ownerPersonId, ownerRoleId );
            }

            return impliedGroupId;
        }

        public int? SaveKnownRelationshipsGroup(int ownerPersonId, int? groupId = null)
        {
            Guid knownRelationshipTypeGuid = new Guid( SystemGuid.GroupType.GROUPTYPE_KNOWN_RELATIONSHIPS );
            int knownRelationshipTypeId = GetGroupTypeByGuid( knownRelationshipTypeGuid ).Id;

            int? knownGroupId = SaveGroup( knownRelationshipTypeId, "Known Relationships", groupId: groupId );

            if ( knownGroupId != null )
            {
                GroupTypeRoleController roleController = new GroupTypeRoleController( Service );
                Guid roleGuid = new Guid( SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_OWNER );
  
                int roleId = roleController.GetByGuid( roleGuid ).Id;

                SaveGroupMember( (int)knownGroupId, ownerPersonId, roleId );

            }

            return knownGroupId;
        }

        public int? SaveGroup( int groupTypeId, string name, int? parentGroupId = null, bool isSystem = false, int? campusId = null, 
                        string description = null, bool isSecurityRole = false, bool isActive = true, int order = 0, string foreignId = null, 
                        int? groupId = null )
        {
            Group group = null;
            GroupController controller = new GroupController( Service );

            if ( groupId != null )
            {
                group = controller.GetById( (int)groupId );
                if ( groupId == null )
                {
                    return null;
                }
            }
            else
            {
                group = new Group();
            }

            group.IsSystem = isSystem;
            group.ParentGroupId = parentGroupId;
            group.GroupTypeId = groupTypeId;
            group.CampusId = campusId;
            group.Name = name;
            group.Description = description;
            group.IsSecurityRole = isSecurityRole;
            group.IsActive = isActive;
            group.Order = order;
            group.ForeignId = foreignId;

            if ( groupId == null )
            {
                group.CreatedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Add( group );
            }
            else
            {
                group.ModifiedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Update( group );
            }


            group = controller.GetByGuid( group.Guid );

            return group.Id;
           
        }

        public int? SaveGroupMember( int groupId, int personId, int groupRoleId, int groupMemberStatus = 0, bool isSystem = false, string foreignId = null, int? groupMemberId = null )
        {
            GroupMember groupMember = null;
            GroupMembersController controller = new GroupMembersController( Service );
            if ( groupMemberId != null )
            {
                groupMember = controller.GetById( (int)groupMemberId );

                if ( groupMember == null )
                {
                    return null;
                }
            }
            else
            {
                groupMember = new GroupMember();
            }

            groupMember.GroupId = groupId;
            groupMember.PersonId = personId;
            groupMember.GroupRoleId = groupRoleId;
            groupMember.GroupMemberStatus = (GroupMemberStatus)groupMemberStatus;
            groupMember.IsSystem = isSystem;
            groupMember.ForeignId = foreignId;

            if ( groupMemberId == null )
            {
                groupMember.CreatedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Add( groupMember );
            }
            else
            {
                groupMember.ModifiedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Update( groupMember );
            }

            groupMember = controller.GetByGuid( groupMember.Guid );

            return groupMember.Id;
        }

        private GroupTypeRole GetGroupTypeRoleByGuid( Guid guid )
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = new TimeSpan( 0, 5, 0 );

            GroupTypeRole  groupTypeRole = cache[string.Format( "GroupTypeRole_{0}", guid )] as GroupTypeRole;

            if ( groupTypeRole == null )
            {
                GroupTypeRoleController controller = new GroupTypeRoleController( Service );
                groupTypeRole = controller.GetByGuid( guid );

                if ( groupTypeRole != null )
                {
                    cache.Set( string.Format( "GroupTypeRole_{0}", guid ), groupTypeRole, policy );
                }
            }

            return groupTypeRole;
        }

        private GroupType GetGroupTypeByGuid( Guid guid )
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = new TimeSpan( 0, 5, 0 );

            GroupType groupType = cache[string.Format( "GroupType_{0}", guid )] as GroupType;

            if ( groupType == null )
            {
                GroupTypeController controller = new GroupTypeController( Service );
                groupType = controller.GetByGuid( guid );

                if ( groupType != null )
                {
                    cache.Set( string.Format( "GroupType_{0}", guid ), groupType, policy );
                }
            }

            return groupType;
        }

    }
}
