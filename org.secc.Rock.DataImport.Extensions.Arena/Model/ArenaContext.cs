using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    public class ArenaContext : DbContext
    {
        public ArenaContext()
            : base( "name=ArenaContext" )
        {
        }

        public ArenaContext( string connectionString )
            : base( connectionString )
        {

        }

        public static ArenaContext BuildContext( Dictionary<string, string> connectionSettings )
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                if ( connectionSettings.ContainsKey( "DatabaseServer" ) )
                {
                    sb.AppendFormat( "Data Source={0};", connectionSettings["DatabaseServer"] );
                }
                else
                {
                    throw new Exception( "Database Server not provided" );
                }

                if ( connectionSettings.ContainsKey( "DatabaseName" ) )
                {
                    sb.AppendFormat( "Initial Catalog={0};", connectionSettings["DatabaseName"] );

                }
                else
                {
                    throw new Exception( "Database Name not provided." );
                }

                bool useIntegratedSecurity = false;

                if ( connectionSettings.ContainsKey( "IntegratedSecurity" ) )
                {
                    bool.TryParse( connectionSettings["IntegratedSecurity"], out useIntegratedSecurity );
                }

                if ( useIntegratedSecurity )
                {
                    sb.Append( "Integrated Security=SSPI;" );
                }
                else
                {
                    if ( connectionSettings.ContainsKey( "UserName" ) )
                    {
                        sb.AppendFormat( "User Id={0};", connectionSettings["UserName"] );
                    }
                    else
                    {
                        throw new Exception( "Database User Name not provided." );
                    }

                    if ( connectionSettings.ContainsKey( "Password" ) )
                    {
                        sb.AppendFormat( "Password={0};", connectionSettings["Password"] );
                    }
                    else
                    {
                        throw new Exception( "Database Password not provided." );
                    }
                }

                sb.Append( "MultipleActiveResultSets=true" );

                return new ArenaContext(sb.ToString());
            }
            catch ( Exception ex )
            {

                throw new Exception( "Connection string invalid", ex );
            }

        }

        //core_address
        public virtual DbSet<Address> Address { get; set; }
        
        //core_attribute
        public virtual DbSet<ArenaAttribute> ArenaAttribute { get; set; }
        
        //core_attribute_group
        public virtual DbSet<ArenaAttributeGroup> ArenaAttributeGroup { get; set; }
        
        //orgn_campus
        public virtual DbSet<Campus> Campus { get; set; }

        //core_family
        public virtual DbSet<Family> Family { get; set; }
        
        //core_family_member
        public virtual DbSet<FamilyMember> FamilyMember { get; set; }
        
        //core_lookup
        public virtual DbSet<Lookup> Lookup { get; set; }
        
        //core_lookup_type
        public virtual DbSet<LookupType> LookupType { get; set; }
        
        //orgn_organization
        public virtual DbSet<Organization> Organization { get; set; }

        //core_person
        public virtual DbSet<Person> Person { get; set; }
        
        //core_person_address
        public virtual DbSet<PersonAddress> PersonAddress { get; set; }
        
        //core_person_attribute
        public virtual DbSet<PersonAttribute> PersonAttribute { get; set; }
        
        //core_person_attribute_history
        public virtual DbSet<PersonAttributeHistory> PersonAttributeHistory { get; set; }
        
        //core_person_email
        public virtual DbSet<PersonEmail> PersonEmail { get; set; }
        
        //core_person_history   
        public virtual DbSet<PersonHistory> PersonHistory { get; set; }
        
        //core_person_merged
        public virtual DbSet<PersonMerged> PersonMerged { get; set; }
        
        //core_person_phone
        public virtual DbSet<PersonPhone> PersonPhone { get; set; }


        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            modelBuilder.Entity<Address>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.street_address_1 )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.street_address_2 )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.city )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.state )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.postal_code )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.standardize_msg )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.geocode_service )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.route )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.dpbc )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.lot )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.endorsement_line )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.container_label )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.country )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.bundle_number )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .Property( e => e.imbc )
                .IsUnicode( false );

            modelBuilder.Entity<Address>()
                .HasMany( e => e.OrganizationAddress )
                .WithOptional( e => e.OrganizationAddress )
                .HasForeignKey( e => e.address_id );

            modelBuilder.Entity<ArenaAttribute>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<ArenaAttribute>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<ArenaAttribute>()
                .Property( e => e.attribute_name )
                .IsUnicode( false );

            modelBuilder.Entity<ArenaAttribute>()
                .Property( e => e.type_qualifier )
                .IsUnicode( false );

            modelBuilder.Entity<ArenaAttribute>()
                .HasMany( e => e.PersonAttribute )
                .WithRequired( e => e.ArenaAttribute )
                .HasForeignKey( e => new { e.attribute_id, e.organization_id } );

            modelBuilder.Entity<ArenaAttribute>()
                .HasMany( e => e.PersonAttributeHistory )
                .WithRequired( e => e.ArenaAttribute )
                .HasForeignKey( e => new { e.attribute_id, e.organization_id } );

            modelBuilder.Entity<ArenaAttributeGroup>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<ArenaAttributeGroup>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<ArenaAttributeGroup>()
                .Property( e => e.group_name )
                .IsUnicode( false );

            modelBuilder.Entity<ArenaAttributeGroup>()
                .HasMany( e => e.ArenaAttribute )
                .WithOptional( e => e.ArenaAttributeGroup )
                .WillCascadeOnDelete();

            modelBuilder.Entity<Family>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<Family>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<FamilyMember>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<FamilyMember>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .Property( e => e.lookup_value )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .Property( e => e.lookup_qualifier )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .Property( e => e.lookup_qualifier2 )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .Property( e => e.lookup_qualifier3 )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .Property( e => e.lookup_qualifier4 )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .Property( e => e.lookup_qualifier5 )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .Property( e => e.lookup_qualifier6 )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .Property( e => e.lookup_qualifier7 )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .Property( e => e.lookup_qualifier8 )
                .IsUnicode( false );

            modelBuilder.Entity<Lookup>()
                .HasKey( e => new { e.lookup_id, e.organization_id } );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.FamilyMemberRole )
                .WithRequired( e => e.Role )
                .HasForeignKey( e => new { e.role_luid, e.organization_id } )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.AddressType )
                .WithRequired( e => e.AddressType )
                .HasForeignKey( e => new { e.address_type_luid, e.organization_id } )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.PersonInactiveReason )
                .WithOptional( e => e.InactiveReason )
                .HasForeignKey( e => new { e.inactive_reason_luid, e.organization_id } );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.PersonMaritalStatus )
                .WithOptional( e => e.MaritalStatus )
                .HasForeignKey( e => new { e.marital_status, e.organization_id } );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.PersonMemberStatus )
                .WithOptional( e => e.MemberStatus )
                .HasForeignKey( e => new { e.member_status, e.organization_id } );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.PersonSuffix )
                .WithOptional( e => e.NameSuffix )
                .HasForeignKey( e => new { e.suffix_luid, e.organization_id } );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.PersonTitle )
                .WithOptional( e => e.NameTitle )
                .HasForeignKey( e => new { e.title_luid, e.organization_id } );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.PersonHistoryType )
                .WithRequired( e => e.HistoryType )
                .HasForeignKey( e => new { e.history_type_luid, e.organization_id } )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.PersonPhoneType )
                .WithRequired( e => e.PhoneType )
                .HasForeignKey( e => new { e.phone_luid, e.organization_id } )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Lookup>()
                .HasMany( e => e.PersonPhoneSMSProvider )
                .WithOptional( e => e.SMSProvider )
                .HasForeignKey( e => new { e.sms_provider_luid, e.organization_id } );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.lookup_type_name )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.lookup_type_desc )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.lookup_category )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.qualifier_1_title )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.qualifier_2_title )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.qualifier_3_title )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.qualifier_4_title )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.qualifier_5_title )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.qualifier_6_title )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.qualifier_7_title )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .Property( e => e.qualifier_8_title )
                .IsUnicode( false );

            modelBuilder.Entity<LookupType>()
                .HasMany( e => e.Lookup )
                .WithRequired( e => e.LookupType )
                .HasForeignKey( e => new { e.lookup_type_id, e.organization_id } )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Person>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<Person>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<Person>()
                .Property( e => e.postal_code )
                .IsUnicode( false );

            modelBuilder.Entity<Person>()
                .Property( e => e.Notes )
                .IsUnicode( false );

            modelBuilder.Entity<Person>()
                .Property( e => e.medical_information )
                .IsUnicode( false );

            modelBuilder.Entity<Person>()
                .Property( e => e.giving_unit_id )
                .IsUnicode( false );

            modelBuilder.Entity<Person>()
                .Property( e => e.first_name_metaphone )
                .IsUnicode( false );

            modelBuilder.Entity<Person>()
                .Property( e => e.nick_name_metaphone )
                .IsUnicode( false );

            modelBuilder.Entity<Person>()
                .Property( e => e.last_name_metaphone )
                .IsUnicode( false );

            modelBuilder.Entity<Person>()
                .HasMany( e => e.CampusLeading )
                .WithOptional( e => e.CampusLeader )
                .HasForeignKey( e => e.leader_person_id );

            modelBuilder.Entity<Person>()
                .HasMany( e => e.OrganizationLeading )
                .WithOptional( e => e.OrganizationLeader )
                .HasForeignKey( e => e.leader_person_id );

            modelBuilder.Entity<PersonAddress>()
                .Property( e => e.from_month_day )
                .IsUnicode( false );

            modelBuilder.Entity<PersonAddress>()
                .Property( e => e.to_month_day )
                .IsUnicode( false );

            modelBuilder.Entity<PersonAddress>()
                .Property( e => e.notes )
                .IsUnicode( false );

            modelBuilder.Entity<PersonAttribute>()
                .Property( e => e.varchar_value )
                .IsUnicode( false );

            modelBuilder.Entity<PersonAttribute>()
                .Property( e => e.decimal_value )
                .HasPrecision( 18, 4 );

            modelBuilder.Entity<PersonAttribute>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<PersonAttribute>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<PersonAttributeHistory>()
                .Property( e => e.varchar_value )
                .IsUnicode( false );

            modelBuilder.Entity<PersonAttributeHistory>()
                .Property( e => e.decimal_value )
                .HasPrecision( 18, 4 );

            modelBuilder.Entity<PersonAttributeHistory>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<PersonAttributeHistory>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<PersonEmail>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<PersonEmail>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<PersonEmail>()
                .Property( e => e.email )
                .IsUnicode( false );

            modelBuilder.Entity<PersonEmail>()
                .Property( e => e.notes )
                .IsUnicode( false );

            modelBuilder.Entity<PersonHistory>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<PersonHistory>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<PersonPhone>()
                .Property( e => e.phone_number )
                .IsUnicode( false );

            modelBuilder.Entity<PersonPhone>()
                .Property( e => e.phone_ext )
                .IsUnicode( false );

            modelBuilder.Entity<PersonPhone>()
                .Property( e => e.phone_number_stripped )
                .IsUnicode( false );

            modelBuilder.Entity<Campus>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<Campus>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<Campus>()
                .Property( e => e.name )
                .IsUnicode( false );

            modelBuilder.Entity<Campus>()
                .Property( e => e.url )
                .IsUnicode( false );

            modelBuilder.Entity<Campus>()
                .Property( e => e.description )
                .IsUnicode( false );

            modelBuilder.Entity<Campus>()
                .HasMany( e => e.Person )
                .WithOptional( e => e.Campus )
                .HasForeignKey( e => e.campus_id );

            modelBuilder.Entity<Organization>()
                .Property( e => e.created_by )
                .IsUnicode( false );

            modelBuilder.Entity<Organization>()
                .Property( e => e.modified_by )
                .IsUnicode( false );

            modelBuilder.Entity<Organization>()
                .Property( e => e.organization_name )
                .IsUnicode( false );

            modelBuilder.Entity<Organization>()
                .Property( e => e.organization_url )
                .IsUnicode( false );

            modelBuilder.Entity<Organization>()
                .Property( e => e.organization_desc )
                .IsUnicode( false );

            modelBuilder.Entity<Organization>()
                .Property( e => e.organization_notes )
                .IsUnicode( false );

            modelBuilder.Entity<Organization>()
                .Property( e => e.organization_code )
                .IsUnicode( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.Address )
                .WithRequired( e => e.Organization )
                .HasForeignKey( e => e.organization_id )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.ArenaAttribute )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.Family )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.FamilyMember )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.Lookup )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.LookupType )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.Person )
                .WithRequired( e => e.Organization )
                .HasForeignKey( e => e.organization_id )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.PersonAddress )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.PersonAttribute )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.PersonAttributeHistory )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.PersonHistory )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.PersonPhone )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );

            modelBuilder.Entity<Organization>()
                .HasMany( e => e.Campus )
                .WithRequired( e => e.Organization )
                .WillCascadeOnDelete( false );
        }
    }
}
