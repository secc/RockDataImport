using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Attribute;
using org.secc.Rock.DataImport.BAL.Helper;
using org.secc.Rock.DataImport.BAL.Integration;
using RockMaps = org.secc.Rock.DataImport.BAL.RockMaps;

using org.secc.Rock.DataImport.Extensions.Arena.Model;

namespace org.secc.Rock.DataImport.Extensions.Arena.Maps
{
    [Export(typeof(iExportMapComponent))]
    [ExportMetadata("Name", "Person")]
    [ExportMetadata("Integration", ArenaIntegration.IDENTIFIER)]
    [ExportMetadata("Description", "People records from ArenaChMS")]
    [DefinedType("Person Title", "4784CD23-518B-43EE-9B97-225BF6E07846", "3394CA53-5791-42C8-B996-1D77C740CF03")]
    [DefinedType( "Name Suffix", "16F85B3C-B3E8-434C-9094-F3D41F87A740", "011E3ABF-8A27-4773-ACC0-7EF017241B69" )]
    [DefinedType("Record Status Reason", "E17D5988-0372-4792-82CF-9E37C79F7319", "011E6A99-2006-4392-B66E-98B6262E8A45")]
    [DefinedType( "Connection Status", "2E6540EA-63F0-40FE-BE50-F2A84735E600", "0B4532DB-3188-40F5-B188-E7E6E4448C85" )]
    [DefinedType( "Marital Status", "B4B92C3F-A935-40E1-A00B-BA484EAD613B", "0AAD26C7-AD9D-4FE8-96B1-C9BCD033BB5B" )]

    
    public class PersonMap : iExportMapComponent
    {
        #region Fields
        private int? mRecordCount;
        private int? mDefinedTypeCount;
        private Dictionary<string,string> ConnectionInfo{get;set;}
        #endregion

        #region Properties
        public int? RecordCount
        {
            get 
            {
                if ( mRecordCount == null )
                {
                    mRecordCount = GetRecordCount();
                }
                return mRecordCount;
            }
        }

        public int? DefinedTypeCount
        {
            get
            {
                if ( mDefinedTypeCount == null )
                {
                    mDefinedTypeCount = GetDefinedTypeCount();
                }

                return mDefinedTypeCount;
            }
        }
        #endregion

        #region Constructors
        private PersonMap() {}

        [ImportingConstructor]
        public PersonMap([Import("ConnectionInfo")] Dictionary<string,string> connectionInfo)
        {
            ConnectionInfo = connectionInfo;
        }
        #endregion

        #region Public Methods
        public List<string> GetSubsetIDs( int startingRecord, int size )
        {
            using ( Model.ArenaContext Context = Model.ArenaContext.BuildContext(ConnectionInfo) )
            {
                var query = Context.Person
                               .OrderBy( p => p.person_id )
                               .Skip( startingRecord );

                if ( size > 0 )
                {
                    query = query.Take( size );
                }

                return query.Select( p => p.person_id.ToString() ).ToList();
            }
        }

        public void ExportRecord( string identifier, RockService service)
        {
            int personId = 0;

            if ( !int.TryParse( identifier, out personId ) )
            {
                OnExportAttemptCompleted( identifier, false );
                return;
            }

            Person arenaPerson = GetArenaPerson( personId );

            if ( arenaPerson == null )
            {
                OnExportAttemptCompleted( identifier, false );
            }

            RockMaps.PersonMap rockPersonMap = new BAL.RockMaps.PersonMap( service );
            Dictionary<string, object> rockPerson = rockPersonMap.GetByForeignId( identifier );
            int? rockPersonId = null;

            if ( rockPerson == null )
            {
                int? rockFamilyId = null;
                rockFamilyId = GetRockFamily( arenaPerson.FamilyMember.FirstOrDefault().family_id, service );

                int recordTypeDV = arenaPerson.business ? rockPersonMap.GetRecordTypeBusiness() : rockPersonMap.GetRecordTypePerson();

                int recordStatus;

                switch ( arenaPerson.record_status )
                {
                    case 0:
                        recordStatus = rockPersonMap.GetRecordStatusIdActive();
                        break;
                    case 1:
                        recordStatus = rockPersonMap.GetRecordStatusIdInactive();
                        break;
                    case 2:
                        recordStatus = rockPersonMap.GetRecordStatusIdPending();
                        break;
                    default:
                        recordStatus = rockPersonMap.GetRecordStatusIdPending();
                        break;
                }

                int? recordStatusReasonId = null;

                if ( arenaPerson.inactive_reason_luid != null )
                {
                    recordStatusReasonId = DefinedValueMatch.GetDefinedValueMatch( arenaPerson.inactive_reason_luid.ToString() );
                }

                bool isDeceased = recordStatusReasonId == rockPersonMap.GetRecordStatusReasonIdDeceased();
                int? connectionStatusValueId = DefinedValueMatch.GetDefinedValueMatch( arenaPerson.member_status.ToString() );
                int? titleValueId = null;

                if ( arenaPerson.title_luid != null )
                {
                    titleValueId = DefinedValueMatch.GetDefinedValueMatch( arenaPerson.title_luid.ToString() );
                }

                string firstName = arenaPerson.first_name;
                string middleName = arenaPerson.middle_name;
                string nickName = arenaPerson.nick_name;
                string lastName = arenaPerson.last_name;

                int? suffixValueId = null;
                if(arenaPerson.suffix_luid != null)
                {
                    suffixValueId = DefinedValueMatch.GetDefinedValueMatch( arenaPerson.suffix_luid.ToString() );
                }

                int? birthDay = null;
                int? birthMonth = null;
                int? birthYear = null;
                if ( arenaPerson.birth_date > new DateTime( 1900, 1, 1 ) )
                {
                    birthDay = arenaPerson.birth_date.Day;
                    birthMonth = arenaPerson.birth_date.Month;
                    birthYear = arenaPerson.birth_date.Year;
                }

                int? gender = null;
                switch ( arenaPerson.gender )
                {
                    case 0:
                        gender = 1;
                        break;
                    case 1:
                        gender = 2;
                        break;
                    case 2:
                        gender = 0;
                        break;
                    default:
                        gender = 0;
                        break;
                }



            }
            else
            {
                rockPersonId = (int?)rockPerson["Id"];
            }
            
        }

        public Dictionary<string, Dictionary<string,object>> GetAttributes( Type attributeType )
        {
            return System.Attribute.GetCustomAttributes( this.GetType() )
                .Where( a => a.GetType() == attributeType )
                .Select( a => new
                {
                    Name = a.GetType().GetProperties().Where( p => p.Name == "Name" ).Select( p => p.GetValue( a ) ).FirstOrDefault().ToString(),
                    Attribute = a.GetType().GetProperties().ToDictionary( p => p.Name, p1 => p1.GetValue( a ) )
                } ).ToDictionary(a => a.Name, a => a.Attribute);
                
        }

        public DefinedTypeSummary GetRockDefinedType( string definedTypeName, RockService service )
        {
            var dtAttribute = GetAttributes( typeof( DefinedTypeAttribute ) )
                                .Where( dta => dta.Key == definedTypeName ).FirstOrDefault();

            if ( dtAttribute.Equals( default( Dictionary<string, Dictionary<string, object>> ) ) )
            {
                return null;
            }

            Guid rockDefinedTypeGuid = new Guid( dtAttribute.Value.Where( p => p.Key == "RockDefinedTypeGuid" ).FirstOrDefault().Value.ToString() );

            BAL.RockMaps.DefinedTypeMap DTMap = new BAL.RockMaps.DefinedTypeMap( service );
            return DTMap.GetDefinedTypeSummary( rockDefinedTypeGuid );
        }

        public DefinedTypeSummary GetSourceDefinedType( string definedTypeName )
        {
            var dtAttribute = GetAttributes( typeof( DefinedTypeAttribute ) )
                                .Where( dta => dta.Key == definedTypeName ).FirstOrDefault();

            if ( dtAttribute.Equals( default( Dictionary<string, Dictionary<string, object>> )  ) )
            {
                return null;
            }

            Guid lookupTypeGuid = new Guid( dtAttribute.Value.Where( p => p.Key == "SourceDefinedTypeIdentifier" ).FirstOrDefault().Value.ToString() );

            using ( Model.ArenaContext Context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return Context.LookupType.Where( lt => lt.guid == lookupTypeGuid )
                            .Select( lt => new DefinedTypeSummary()
                                            {
                                                Id = lt.lookup_type_id.ToString(),
                                                Name = lt.lookup_type_name,
                                                Description = lt.lookup_type_desc,
                                                UniqueIdentifier = lt.guid,
                                                ValueSummaries = lt.Lookup.Select( l => new DefinedValueSummary()
                                                        {
                                                            Id = l.lookup_id.ToString(),
                                                            DefinedTypeId = l.lookup_type_id.ToString(),
                                                            Value = l.lookup_value,
                                                            Order = l.lookup_order
                                                        } ).ToList()

                                            } ).First();
            }
        }

        public virtual void OnExportAttemptCompleted( string identifier, bool isSuccess, int? rockId = null )
        {
            ExportMapEventArgs args = new ExportMapEventArgs();
            args.Identifier = identifier;
            args.RockIdentifier = rockId;
            args.IsSuccess = isSuccess;

            EventHandler<ExportMapEventArgs> handler = ExportAttemptCompleted;

            if ( handler != null )
            {
                handler( this, args );
            }
        }

        #endregion

        public event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;

        #region Private Methods

        private Person GetArenaPerson( int personId )
        {
            using ( ArenaContext Context = ArenaContext.BuildContext( ConnectionInfo ) )
            {
               
                return Context.Person
                        .Include( "PersonEmail" )
                        .Include( "PersonAddress.Address" )
                        .Include( "FamilyMember.Family" )
                        .FirstOrDefault( p => p.person_id.Equals( personId ) );
            }
        }

        private int GetDefinedTypeCount()
        {
            return System.Attribute.GetCustomAttributes( this.GetType() )
                    .Where( a => a.GetType() == typeof( DefinedTypeAttribute ) ).Count();
        }

        private int? GetRecordCount()
        {
            using (Model.ArenaContext Context = Arena.Model.ArenaContext.BuildContext(ConnectionInfo))
            {
                return Context.Person.Count();
            }
        }

        private int? GetRockFamily( int arenaFamilyId, RockService service )
        {
            RockMaps.GroupMap groupMap = new RockMaps.GroupMap( service );
            Dictionary<string, object> rockFamily = groupMap.GetFamilyGroupByForeignId( arenaFamilyId.ToString() );

            if ( rockFamily != null )
            {
                return (int?)rockFamily["Id"];
            }
            else
            {
                Family arenaFamily = GetArenaFamily(arenaFamilyId);

                if(arenaFamily == null)
                {
                    return null;
                }

                int? arenaCampusId = GetArenaFamilyCampusId( arenaFamilyId );
                int? rockCampusId = (int?) (new RockMaps.CampusMap(service).GetByForeignId( arenaCampusId.ToString() )["Id"]);
                int? rockFamilyId = groupMap.SaveFamily( rockCampusId, arenaFamily.family_name, arenaFamilyId.ToString() );

                return rockFamilyId;
            }
        }

        private Family GetArenaFamily( int familyId )
        {
            using ( ArenaContext context = ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return context.Family.FirstOrDefault( f => f.family_id == familyId );
            }
        }

        private int? GetArenaFamilyCampusId( int arenaFamilyId )
        {
            //determine HoH
            using ( ArenaContext context = ArenaContext.BuildContext( ConnectionInfo ) )
            {
                int adultRoleId = context.Lookup.FirstOrDefault( l => l.guid == new Guid( Lookup.FAMILY_MEMBER_ROLE_ADULT_GUID ) ).lookup_id;
                var familyMembers = context.FamilyMember
                                        .Include("Person")
                                        .Where( fm => fm.family_id == arenaFamilyId );

                FamilyMember hoh = familyMembers
                            .Where( fm => fm.role_luid == adultRoleId )
                            .OrderBy( fm => fm.Person.gender )
                            .FirstOrDefault();

                if ( hoh == null )
                {
                    hoh = familyMembers.OrderBy( fm => fm.Person.birth_date ).FirstOrDefault();
                }

                return hoh.Person.campus_id;
            }
        }

        #endregion
    }
}
