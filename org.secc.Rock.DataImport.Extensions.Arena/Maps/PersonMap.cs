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
    [ExportMetadata("ImportRanking", 1)]
    [DefinedType("Person Title", "4784CD23-518B-43EE-9B97-225BF6E07846", "3394CA53-5791-42C8-B996-1D77C740CF03")]
    [DefinedType( "Name Suffix", "16F85B3C-B3E8-434C-9094-F3D41F87A740", "011E3ABF-8A27-4773-ACC0-7EF017241B69" )]
    [DefinedType("Record Status Reason", "E17D5988-0372-4792-82CF-9E37C79F7319", "011E6A99-2006-4392-B66E-98B6262E8A45")]
    [DefinedType( "Connection Status", "2E6540EA-63F0-40FE-BE50-F2A84735E600", "0B4532DB-3188-40F5-B188-E7E6E4448C85" )]
    [DefinedType( "Marital Status", "B4B92C3F-A935-40E1-A00B-BA484EAD613B", "0AAD26C7-AD9D-4FE8-96B1-C9BCD033BB5B" )]
    [DefinedType( "Phone Type", "8345DD45-73C6-4F5E-BEBD-B77FC83F18FD", "847C4CB1-0C3F-4B9C-AA97-DC1A5AFEE26B" )]
    [DefinedType( "Address Type", "2E68D37C-FB7B-4AA5-9E09-3785D52156CB", "9B4BE12C-C105-4F80-8254-8639B27D7640" )]
    [Dependency("Campus", typeof(CampusMap))]

    
    public class PersonMap : ArenaMapBase
    {
        #region Fields



        private const int ARENA_ADULT_ROLE_LUID = 29;
        private const int ARENA_CHILD_ROLE_LUID = 31;

        #endregion

        #region Properties
        public override int? RecordCount
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

        #endregion

        #region Constructors
        private PersonMap(): base( typeof( PersonMap ) ) {}

        [ImportingConstructor]
        public PersonMap( [Import( "ConnectionInfo" )] Dictionary<string, string> connectionInfo, [Import("RockService")] RockService service ) : base( typeof( PersonMap ), connectionInfo, service ) { }
        #endregion

        #region Public Methods
        public override List<string> GetSubsetIDs( int startingRecord, int size )
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

        public override void ExportRecord( string identifier)
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
                return;
            }

            RockMaps.PersonMap rockPersonMap = new BAL.RockMaps.PersonMap( Service );
            Dictionary<string, object> rockPerson = rockPersonMap.GetByForeignId( identifier );

            int? rockPersonId = null;

            if ( rockPerson == null )
            {
                int? rockFamilyId = GetRockFamily( arenaPerson.FamilyMember.FirstOrDefault().Family );

                int recordTypeValueId = arenaPerson.business ? rockPersonMap.GetRecordTypeBusiness() : rockPersonMap.GetRecordTypePerson();

                int recordStatusValueId;

                switch ( arenaPerson.record_status )
                {
                    case 0:
                        recordStatusValueId = rockPersonMap.GetRecordStatusIdActive();
                        break;
                    case 1:
                        recordStatusValueId = rockPersonMap.GetRecordStatusIdInactive();
                        break;
                    case 2:
                        recordStatusValueId = rockPersonMap.GetRecordStatusIdPending();
                        break;
                    default:
                        recordStatusValueId = rockPersonMap.GetRecordStatusIdPending();
                        break;
                }

                int? recordStatusReasonId = null;

                if ( arenaPerson.inactive_reason_luid != null )
                {
                    recordStatusReasonId = GetDefinedValueIdByForeignId( arenaPerson.inactive_reason_luid );

                }

                bool isDeceased = recordStatusReasonId == rockPersonMap.GetRecordStatusReasonIdDeceased();

                int? connectionStatusValueID = GetDefinedValueIdByForeignId( arenaPerson.member_status ); 

                int? titleValueId = null;

                if ( arenaPerson.title_luid != null )
                {
                    titleValueId = GetDefinedValueIdByForeignId( arenaPerson.title_luid );
                }

                int? suffixValueId = null;

                if ( arenaPerson.suffix_luid != null )
                {
                    suffixValueId = GetDefinedValueIdByForeignId( arenaPerson.suffix_luid );  
                }

                int? birthYear = null;
                int? birthMonth = null;
                int? birthDay = null;

                if ( arenaPerson.birth_date != null && arenaPerson.birth_date > new DateTime( 1900, 1, 1 ) )
                {
                    birthYear = arenaPerson.birth_date.Year;
                    birthMonth = arenaPerson.birth_date.Month;
                    birthDay = arenaPerson.birth_date.Day;
                }

                int gender;

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

                int? maritalStatusValueId = null;

                if ( arenaPerson.marital_status != null )
                {
                    maritalStatusValueId = GetDefinedValueIdByForeignId( arenaPerson.marital_status );
                }

                DateTime? anniversaryDate = null;

                if ( arenaPerson.anniversary_date != null && arenaPerson.anniversary_date > new DateTime( 1900, 1, 1 ) )
                {
                    anniversaryDate = arenaPerson.anniversary_date;
                }

                DateTime? graduationDate = null;

                if ( arenaPerson.graduation_date != null && arenaPerson.graduation_date > new DateTime( 1900, 1, 1 ) )
                {
                    graduationDate = arenaPerson.graduation_date;
                }

                int? givingGroupId = null;

                if ( !arenaPerson.contribute_individually )
                {
                    givingGroupId = rockFamilyId;
                }

                string primaryEmailAddress = null;
                string emailNote = null;
                bool isEmailActive = false;
                int? emailPreference = null;

                if ( arenaPerson.PersonEmail.Count > 0 )
                {
                    int minOrder = arenaPerson.PersonEmail.Min( pe => pe.email_order );

                    PersonEmail arenaPrimaryEmail = arenaPerson.PersonEmail.FirstOrDefault( e => e.email_order == minOrder );
                    primaryEmailAddress = arenaPrimaryEmail.email;
                    emailNote = arenaPrimaryEmail.notes;
                    isEmailActive = arenaPrimaryEmail.active;

                    if ( isEmailActive )
                    {
                        if ( arenaPrimaryEmail.allow_bulk_mail )
                        {
                            emailPreference = 0;
                        }
                        else
                        {
                            emailPreference = 1;
                        }
                    }
                    else
                    {
                        emailPreference = 2;
                    }
                }

                string foreignId = arenaPerson.person_id.ToString();

                int? photoId = null;

                if ( arenaPerson.blob_id != null && PhotoUploadEnabled() )
                {
                    photoId = SavePersonPhoto( arenaPerson.Blob );
                }


                rockPersonId = rockPersonMap.Save( false, recordTypeValueId: recordTypeValueId, recordStatusValueId: recordStatusValueId, recordStatusReasonValueId: recordStatusReasonId, isDeceased: isDeceased,
                    connectionStatusValueId: connectionStatusValueID, titleValueId: titleValueId, firstName: arenaPerson.first_name, nickName: arenaPerson.nick_name, middleName: arenaPerson.middle_name,
                    lastName: arenaPerson.last_name, suffixValueId: suffixValueId, birthDay: birthDay, birthMonth: birthMonth, birthYear: birthYear, gender: gender, maritalStatusValueId: maritalStatusValueId,
                    anniversaryDate: anniversaryDate, graduationDate: graduationDate, givingGroupId: givingGroupId, email: primaryEmailAddress, isEmailActive: isEmailActive, emailNote: emailNote, emailPreference: emailPreference,
                    foreignId: foreignId, photoId: photoId );

                if ( rockPersonId == null )
                {
                    OnExportAttemptCompleted( identifier, false );
                    return;
                }

                int? personAliasId = rockPersonMap.SaveNewPersonAlias( (int)rockPersonId );

                RockMaps.GroupMap rockGroupMap = new RockMaps.GroupMap( Service );
                int familyMemberRoleId;

                if ( arenaPerson.FamilyMember.FirstOrDefault().role_luid == ARENA_ADULT_ROLE_LUID )
                {
                    familyMemberRoleId = rockGroupMap.GetFamilyAdultGroupRoleId();
                }
                else
                {
                    familyMemberRoleId = rockGroupMap.GetFamilyChildGroupRoleId();
                }

                

                rockGroupMap.SaveGroupMember( (int) rockFamilyId, (int) rockPersonId, familyMemberRoleId, 1 );

                rockGroupMap.SaveKnownRelationshipsGroup( (int)rockPersonId );
                rockGroupMap.SaveImpliedRelationshipsGroup( (int)rockPersonId );

                int? individualRockFamilyId = null;
                foreach ( var personAddress in arenaPerson.PersonAddress )
                {


                    if ( AddressIsFamilyAddress( arenaPerson.FamilyMember.FirstOrDefault().family_id, personAddress.address_id ) )
                    {
                        //either a single member family or Arena address found on multiple people in the family.
                        AddFamilyLocation( (int) rockFamilyId, personAddress );
                    }
                    else
                    {
                        //multiple person family and address only listed for this person
                        if ( individualRockFamilyId == null )
                        {

                            individualRockFamilyId = AddIndividualFamily( (int) rockPersonId, arenaPerson,  (int) rockFamilyId );
                        }
                        AddFamilyLocation( (int) individualRockFamilyId, personAddress );
                    }
                }

                foreach ( var phone in arenaPerson.PersonPhone.Where(p => !String.IsNullOrWhiteSpace(p.phone_number) ) )
                {
                    int? personPhone = SavePersonPhone( (int) rockPersonId, phone );
                }



            }
            else
            {
                rockPersonId = (int?)rockPerson["Id"];
            }


            if ( rockPersonId != null )
            {
                OnExportAttemptCompleted( identifier, true, rockPersonId, this.GetType() );
            }
            else
            {
                OnExportAttemptCompleted( identifier, false, mapType: this.GetType() );
            }

        }

        public override Dictionary<string, Dictionary<string,object>> GetAttributes( Type attributeType )
        {
            return GetAttributes( this.GetType(), attributeType );
                
        }

        public virtual void OnExportAttemptCompleted( string identifier, bool isSuccess, int? rockId = null, Type mapType = null )
        {
            ExportMapEventArgs args = new ExportMapEventArgs();
            args.Identifier = identifier;
            args.RockIdentifier = rockId;
            args.IsSuccess = isSuccess;

            if ( mapType == null )
            {
                args.MapType = this.GetType();
            }
            else
            {
                args.MapType = mapType;
            }

            EventHandler<ExportMapEventArgs> handler = ExportAttemptCompleted;

            if ( handler != null )
            {
                handler( this, args );
            }
        }

        #endregion

        public override event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;

        #region Private Methods

        private int? AddIndividualFamily( int? rockPersonId, Person arenaPerson, int modelRockFamilyId )
        {
            RockMaps.GroupMap groupMap = new RockMaps.GroupMap( Service );

            Dictionary<string, object> modelFamily = groupMap.GetGroupById( modelRockFamilyId );
            var modelFamilyMembers = groupMap.GetGroupMemberByGroupIdPersonId( (int)modelFamily["Id"], (int)rockPersonId );

            int roleId = 0;

            if ( modelFamilyMembers != null && modelFamilyMembers.Count > 0 )
            {
                roleId = (int)modelFamilyMembers.First().Value["GroupRoleId"];
            }


            string groupDescription = string.Format( "Individual \"family\" for {0} {1}", arenaPerson.nick_name, arenaPerson.last_name );
            int? individualFamilyId = groupMap.SaveFamily( (int?)modelFamily["CampusId"], modelFamily["Name"].ToString(), description: groupDescription );

            if ( individualFamilyId != null )
            {
                groupMap.SaveGroupMember( (int)individualFamilyId, (int)rockPersonId, roleId );
            }

            return individualFamilyId;
        }

        private void AddFamilyLocation( int rockFamilyId, PersonAddress personAddress)
        {
            RockMaps.LocationMap locationMap = new RockMaps.LocationMap( Service );
            Address arenaAddress = personAddress.Address;
            int? rockLocationId = null;
            Dictionary<string, object> rockLocation = locationMap.GetByForeignId( arenaAddress.address_id.ToString() );

            if ( rockLocation != null )
            {
                rockLocationId = (int)rockLocation["Id"];
            }
            else
            {
                rockLocationId = locationMap.SaveAddress( arenaAddress.street_address_1, arenaAddress.city, arenaAddress.state, arenaAddress.country,
                        arenaAddress.postal_code, arenaAddress.street_address_2, arenaAddress.Latitude, arenaAddress.Longitude, arenaAddress.address_id.ToString(), isActive: true );
            }

            if ( rockLocationId != null )
            {
                RockMaps.GroupLocationMap glMap = new RockMaps.GroupLocationMap( Service );

                if ( glMap.GetGroupLocationByGroupIdLocationId( rockFamilyId, (int)rockLocationId ).Count > 0 )
                {
                    return;
                }
                else
                {
                    glMap.SaveGroupLocation( rockFamilyId, (int)rockLocationId, GetDefinedValueIdByForeignId(personAddress.address_type_luid), null, true, true );
                }
            }

        }

        private bool AddressIsFamilyAddress( int familyId, int addressId )
        {
            using ( ArenaContext context = ArenaContext.BuildContext( ConnectionInfo ) )
            {
                bool isFamilyAddress = true;
                var familyMemberIds = context.FamilyMember.Where( fm => fm.family_id == familyId ).Select( fm => fm.person_id ).ToList();

                if ( familyMemberIds.Count > 1 )
                {
                    //isFamilyAddress = context.PersonAddress.Where( pa => pa.address_id == addressId ).Where( pa => familyMemberIds.Contains( pa.person_id ) ).Count() > 1;

                    var familyMemberAddresses = context.PersonAddress.Where( pa => familyMemberIds.Contains( pa.person_id ) ).Select( pa => new { pa.person_id, pa.address_id } ).Distinct().ToList();

                    isFamilyAddress = familyMemberAddresses.Where( a => a.address_id == addressId ).Count() > 1;
                }

                return isFamilyAddress;
            }
        }

        private bool PhotoUploadEnabled()
        {
            return new RockMaps.BinaryFileTypeMap( Service ).IsPhotoUploadEnabled();
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
                                        .Include( "Person" )
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

        private Person GetArenaPerson( int personId )
        {
            using ( ArenaContext Context = ArenaContext.BuildContext( ConnectionInfo ) )
            {

                return Context.Person
                        .Include( "PersonEmail" )
                        .Include( "PersonPhone" )
                        .Include( "PersonAddress.Address" )
                        .Include( "FamilyMember.Family" )
                        .Include( "Blob" )
                        .FirstOrDefault( p => p.person_id.Equals( personId ) );
            }
        }



        private int? GetRecordCount()
        {
            using (Model.ArenaContext Context = Arena.Model.ArenaContext.BuildContext(ConnectionInfo))
            {
                return Context.Person.Count();
            }
        }

        private int? GetRockFamily( Model.Family f )
        {
            RockMaps.GroupMap groupMap = new RockMaps.GroupMap( Service );
            Dictionary<string, object> rockFamily = groupMap.GetFamilyGroupByForeignId( f.family_id.ToString() );

            if ( rockFamily != null )
            {
                return (int?)rockFamily["Id"];
            }
            else
            {
                if(f == null)
                {
                    return null;
                }

                int? arenaCampusId = GetArenaFamilyCampusId( f.family_id );
                int? rockCampusId = (int?) (new RockMaps.CampusMap(Service).GetByForeignId( arenaCampusId.ToString() )["Id"]);
                int? rockFamilyId = groupMap.SaveFamily( rockCampusId, f.family_name, null, f.family_id.ToString() );

                return rockFamilyId;
            }
        }



        private int? SavePersonPhone( int rockPersonId, PersonPhone phone )
        {
            RockMaps.PhoneNumberMap rockPhoneMap = new RockMaps.PhoneNumberMap( Service );

            var rockPhone = rockPhoneMap.GetPhoneByForeignId( string.Format( "{0}_{1}", phone.person_id, phone.phone_luid ) );
            int? rockPhoneId = null;
            if ( rockPhone == null )
            {
                rockPhoneId = rockPhoneMap.SavePhone( number: phone.phone_number_stripped, personId: rockPersonId, numbertypeValueId: (int) GetDefinedValueIdByForeignId(phone.phone_luid), 
                    extension: phone.phone_ext, isSystem: false, isMessagingEnabled: phone.sms_enabled, isUnlisted: phone.unlisted, foreignId: string.Format( "{0}_{1}", phone.person_id, phone.phone_luid ) );
            }
            else
            {
                rockPhoneId = (int)rockPhone["Id"];
            }

            return rockPhoneId;
        }

        private int? SavePersonPhoto( Blob photoBlob )
        {
            RockMaps.BinaryFileMap binaryFileMap = new RockMaps.BinaryFileMap( Service );
            Dictionary<string, object> binaryFile = binaryFileMap.GetByForeignId( photoBlob.blob_id.ToString() );

            if ( binaryFile != null )
            {
                return (int?)binaryFile["Id"];
            }
            string fileName = string.Empty;

            if ( String.IsNullOrWhiteSpace( photoBlob.original_file_name ) )
            {
                fileName = string.Format( "{0}.{1}", photoBlob.blob_id, photoBlob.mime_type.Replace( "image/", "" ) );
            }
            else
            {
                fileName = photoBlob.original_file_name;
            }

            int? fileId = binaryFileMap.SavePersonPhoto( fileName , photoBlob.mime_type, photoBlob.description, photoBlob.blob, false, false, photoBlob.blob_id.ToString() );

            return fileId;
        }

        #endregion
    }
}
