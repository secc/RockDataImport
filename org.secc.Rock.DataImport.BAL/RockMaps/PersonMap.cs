using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using SystemGuid = Rock.SystemGuid;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class PersonMap : MapBase
    {
        RockService Service { get; set; }

        private PersonMap() { }

        public PersonMap( RockService service )
        {
            Service = service;
        }

        public Dictionary<string, object> GetByForeignId( string foreignId, bool includeAlias = false )
        {
            PersonController controller = new PersonController( Service );
            Person person = controller.GetByForeignId( foreignId );
            Dictionary<string, object> personDictionary = ToDictionary( person );

            if ( person != null && includeAlias )
            {
                PersonAlias primaryAlias = GetPrimaryAlias( person.Id );
                if(primaryAlias != null)
                {
                    personDictionary.Add( "PrimaryAliasId", primaryAlias.Id );
                }
                
            }

            return personDictionary;
        }

        public Dictionary<string, object> GetById( int id, bool includeAlias = false )
        {
            PersonController controller = new PersonController( Service );
            Person person = controller.GetById( id );

            Dictionary<string, object> personDictionary = ToDictionary( person );

            if ( person != null && includeAlias )
            {
                PersonAlias primaryAlias = GetPrimaryAlias( person.Id );
                if ( primaryAlias != null )
                {
                    personDictionary.Add( "PrimaryAliasId", primaryAlias.Id );
                }

            }

            return personDictionary;
        }

        public PersonAlias GetPrimaryAlias( int personId )
        {
            PersonAliasController aliasController = new PersonAliasController( Service );
            List<PersonAlias> aliases = aliasController.GetByPersonId( personId );

            return aliases.FirstOrDefault( a => a.AliasPersonId == personId );
        }

        public int GetRecordStatusIdActive()
        {
            DefinedValueMap dvm = new DefinedValueMap( Service );
            Guid recordStatusActiveGuid = new Guid( SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE );
            return dvm.GetDefinedValueByGuid( recordStatusActiveGuid ).Id;
        }

        public int GetRecordStatusIdInactive()
        {
            DefinedValueMap dvm = new DefinedValueMap( Service );
            Guid recordStatusInactiveGuid = new Guid( SystemGuid.DefinedValue.PERSON_RECORD_STATUS_INACTIVE );
            return dvm.GetDefinedValueByGuid( recordStatusInactiveGuid ).Id;
        }

        public int GetRecordStatusIdPending()
        {
            DefinedValueMap dvm = new DefinedValueMap( Service );
            Guid recordStatusPendingGuid = new Guid( SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING );
            return dvm.GetDefinedValueByGuid( recordStatusPendingGuid ).Id;
        }

        public int GetRecordStatusReasonIdDeceased()
        {
            DefinedValueMap dvm = new DefinedValueMap( Service );
            Guid reasonDeceasedGuid = new Guid( "05D35BC4-5816-4210-965F-1BF44F35A16A" );
            return dvm.GetDefinedValueByGuid( reasonDeceasedGuid ).Id;  
        }

        public int GetRecordTypeBusiness()
        {
            DefinedValueMap dvm = new DefinedValueMap( Service );
            Guid businessGuid = new Guid( SystemGuid.DefinedValue.PERSON_RECORD_TYPE_BUSINESS );
            return dvm.GetDefinedValueByGuid( businessGuid ).Id;
        }

        public int GetRecordTypePerson()
        {
            DefinedValueMap dvm = new DefinedValueMap( Service );
            Guid personGuid = new Guid( SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON );
            return dvm.GetDefinedValueByGuid( personGuid ).Id;
        }


        public int? Save(bool isSystem, int? recordTypeValueId = null, int? recordStatusValueId = 0, int? recordStatusReasonValueId = null, int? connectionStatusValueId = null, bool isDeceased = false, 
                int? titleValueId = null, string firstName = null, string nickName = null, string middleName = null, string lastName = null, int? suffixValueId = null, int? photoId = null, int? birthDay = null,
                int? birthMonth = null, int? birthYear = null, int gender = 0, int? maritalStatusValueId = null, DateTime? anniversaryDate = null, DateTime? graduationDate = null, int? givingGroupId = null, 
                string email= null, bool isEmailActive = false, string emailNote = null, string systemNote = null, int? emailPreference = 2, string inactiveReasonNote = null, string foreignId = null, 
                int? reviewReasonValueId = null, string reviewReasonNote = null, int? personId = null)
        {
            Person p;
            PersonController controller = new PersonController( Service );
            if ( personId > 0 )
            {
                p = controller.GetById( (int)personId );

                if ( p == null )
                {
                    return null;
                }
            }
            else
            {
                p = new Person();
            }

            p.IsSystem = isSystem;
            p.RecordTypeValueId = recordTypeValueId;
            p.RecordStatusValueId = recordStatusValueId;
            p.RecordStatusReasonValueId = recordStatusReasonValueId;
            p.ConnectionStatusValueId = connectionStatusValueId;
            p.IsDeceased = isDeceased;
            p.TitleValueId = titleValueId;
            p.FirstName = firstName;
            p.NickName = nickName;
            p.MiddleName = middleName;
            p.LastName = lastName;
            p.SuffixValueId = suffixValueId;
            p.PhotoId = photoId;
            p.BirthDay = birthDay;
            p.BirthMonth = birthMonth;
            p.BirthYear = birthYear;
            p.Gender = (Gender)gender;
            p.MaritalStatusValueId = maritalStatusValueId;
            p.AnniversaryDate = anniversaryDate;
            p.GraduationDate = graduationDate;
            p.GivingGroupId = givingGroupId;
            p.Email = email;
            p.IsEmailActive = isEmailActive;
            p.EmailNote = emailNote;
            p.SystemNote = systemNote;
            p.EmailPreference = (EmailPreference)emailPreference;
            p.InactiveReasonNote = inactiveReasonNote;
            p.ForeignId = foreignId;
            p.ReviewReasonValueId = reviewReasonValueId;
            p.ReviewReasonNote = reviewReasonNote;

            if ( p.Id == 0 )
            {
                p.CreatedByPersonAliasId = Service.LoggedInPerson.PrimaryAliasId;
                controller.Add( p );
            }
            else
            {
                p.ModifiedByPersonAliasId = Service.LoggedInPerson.PrimaryAliasId;
                controller.Update( p );
            }

            p = controller.GetByGuid( p.Guid );

            return p.Id;
        }

        public int? SaveNewPersonAlias( int personId)
        {
            PersonAliasController aliasController = new PersonAliasController( Service );
            string expression = string.Format( "PersonId eq {0} and AliasPersonId eq {1}", personId, personId );
            PersonAlias alias = aliasController.GetByFilter( expression ).FirstOrDefault();

            PersonController personController = new PersonController( Service );
            Person person = personController.GetById( personId );


            if ( alias == null )
            {

                alias = new PersonAlias();
                alias.PersonId = personId;
                alias.AliasPersonId = personId;
                alias.AliasPersonGuid = person.Guid;

                aliasController.Add( alias );

                return aliasController.GetByGuid( alias.Guid ).Id;
            }
            else
            {
                return alias.Id;
            }
        }
    }
}
