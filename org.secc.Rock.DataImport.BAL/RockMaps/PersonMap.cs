using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class PersonMap
    {
        RockService Service { get; set; }

        private PersonMap() { }

        public PersonMap( RockService service )
        {
            Service = service;
        }

        public Dictionary<string, object> GetByForeignId( string foreignId )
        {
            PersonController controller = new PersonController( Service );
            Person person = controller.GetByForeignId( foreignId );
            Dictionary<string, object> personDictionary = ToDictionary( person );

            return personDictionary;
        }

        public Dictionary<string, object> GetById( int id )
        {
            PersonController controller = new PersonController( Service );
            Person person = controller.GetById( id );
            Dictionary<string, object> personDictionary = ToDictionary( person );

            return personDictionary;
        }

        public int? Save(bool isSystem, int? recordTypeValueId = null, int? recordStatusValueId = 0, int? recordStatusReasonValueId = null, int? connectionStatusValueId = null, bool isDeceased = false, 
                int? titleValueId = null, string firstName = null, string nickName = null, string middleName = null, string lastName = null, int? suffixValueId = null, int? photoId = null, int? birthDay = null,
                int? birthMonth = null, int? birthYear = null, int gender = 0, int? maritalStatusValueId = null, DateTime? anniversaryDate = null, DateTime? graduationDate = null, int? givingGroupId = null, 
                string email= null, bool isEmailActive = false, string emailNote = null, string systemNote = null, int emailPreference = 2, string inactiveReasonNote = null, string foreignId = null, 
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
                p.CreatedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Add( p );
            }
            else
            {
                p.ModifiedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Update( p );
            }

            p = controller.GetByGuid( p.Guid );

            return p.Id;


        }
            

        private Dictionary<string,object> ToDictionary(Person p)
        {
                Dictionary<string, object> personDictionary = null;

                if ( p != null )
                {
                    personDictionary = p.ToDictionary();

                    if ( !personDictionary.ContainsKey( "CreatedByPersonAliasId" ) )
                    {
                        personDictionary.Add( "CreatedByPersonAliasId", p.CreatedByPersonAliasId );
                    }

                    if ( !personDictionary.ContainsKey( "ModifiedByPersonAliasId" ) )
                    {
                        personDictionary.Add( "ModifiedByPersonAliasId", p.ModifiedByPersonAliasId );
                    }

                    if ( !personDictionary.ContainsKey( "CreatedDateTime" ) )
                    {
                        personDictionary.Add( "CreatedDateTime", p.CreatedDateTime );
                    }

                    if ( !personDictionary.ContainsKey( "ModifiedDateTime" ) )
                    {
                        personDictionary.Add( "ModifiedDateTime", p.ModifiedDateTime );
                    }
                }

            return personDictionary;
        }
    }
}
