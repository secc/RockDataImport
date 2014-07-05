using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;
using SysteGuid = Rock.SystemGuid;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class PhoneNumberMap
    {
        RockService Service { get; set; }

        private PhoneNumberMap() { }
        
        public PhoneNumberMap( RockService service )
        {
            Service = service;
        }

        public int? SavePhone(string number, int personId, int numbertypeValueId, string extension = null, bool isSystem = false, bool isMessagingEnabled = false, 
                bool isUnlisted = false, string description = null, string countryCode = "1", string foreignId = null, int? phoneNumberId = null )
        {
            PhoneNumber phone = null;
            PhoneNumberController controller = new PhoneNumberController( Service );

            if ( phoneNumberId != null )
            {
                phone = controller.GetById( (int)phoneNumberId );

                if ( phone == null )
                {
                    return null;
                }
            }
            else
            {
                phone = new PhoneNumber();
            }

            string cleanPhone = PhoneNumber.CleanNumber( number );

            phone.PersonId = personId;
            phone.Number = cleanPhone;
            phone.Extension = extension;
            phone.CountryCode = countryCode;
            phone.NumberTypeValueId = numbertypeValueId;
            phone.IsSystem = isSystem;
            phone.IsMessagingEnabled = isMessagingEnabled;
            phone.IsUnlisted = isUnlisted;
            phone.Description = description;
            phone.ForeignId = foreignId;
            phone.NumberFormatted = System.Text.RegularExpressions.Regex.Replace( cleanPhone, @"^(\d{3})(\d{3})(\d{4})$", @"($1) $2-$3" );

            return SavePhone( phone );
        }

        public Dictionary<string, object> GetPhoneByForeignId( string foreignId )
        {
            PhoneNumberController controller = new PhoneNumberController( Service );

            var phoneNumber = controller.GetByForeignId( foreignId );

            return ToDictionary( phoneNumber );
        }

        private PhoneNumber GetPhoneById( int id )
        {
            PhoneNumberController controller = new PhoneNumberController( Service );

            return controller.GetById( id );
        }

        private int? SavePhone( PhoneNumber p )
        {
            PhoneNumberController controller = new PhoneNumberController( Service );

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

            return controller.GetByGuid( p.Guid ).Id;
        }

        private Dictionary<string, object> ToDictionary( PhoneNumber p )
        {
            Dictionary<string, object> phoneDictionary = null;

            if ( p != null )
            {
                phoneDictionary = p.ToDictionary();

                if ( !phoneDictionary.ContainsKey( "CreatedByPersonAliasId" ) )
                {
                    phoneDictionary.Add( "CreatedByPersonAliasId", p.CreatedByPersonAliasId );
                }

                if ( !phoneDictionary.ContainsKey( "ModifiedByPersonAliasId" ) )
                {
                    phoneDictionary.Add( "ModifiedByPersonAliasId", p.ModifiedByPersonAliasId );
                }

                if ( !phoneDictionary.ContainsKey( "CreatedDateTime" ) )
                {
                    phoneDictionary.Add( "CreatedDateTime", p.CreatedDateTime );
                }

                if ( !phoneDictionary.ContainsKey( "ModifiedDateTime" ) )
                {
                    phoneDictionary.Add( "ModifiedDateTime", p.ModifiedDateTime );
                }
            }

            return phoneDictionary;
        }

    }
}
