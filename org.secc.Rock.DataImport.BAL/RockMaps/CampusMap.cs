using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class CampusMap
    {
        RockService Service { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="CampusMap"/> class from being created.
        /// </summary>
        private CampusMap()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CampusMap"/> class.
        /// </summary>
        /// <param name="service">The <see cref="RockService"/>.</param>
        public CampusMap(RockService service)
        {
            Service = service;

        }

        /// <summary>
        /// Saves a Campus to the Rock instance.  If the <see cref="campusId"/> is null, an add will be attempted, otherwise an update will be attempted.
        /// </summary>
        /// <param name="isSystem">A <see cref="System.Boolean"/> value that is <c>true<c> if the Campus is a system generated value.</param>
        /// <param name="name">A <see cref="System.String"/> representing the unique name of the campus..</param>
        /// <param name="shortCode">A <see cref="System.String"/> representing the optional short code for the campus. Defaults to null.</param>
        /// <param name="locationId">A nullable <see cref="System.Int32"/> that represents the LcoationId  of the <see cref="Rock.Model.Location"/> that is associated with the campus. Defaults to null.</param>
        /// <param name="phoneNumber">A <see cref="System.String"/> representing the phone number of the campus. Defaults to null.</param>
        /// <param name="leaderPersonAliasId">A <see cref="System.Int32"/> that represents the PersonAliasId of a <see cref="Rock.Model.PersonAlias"/> that is associated with the <see cref="Rock.Model.Person"/>
        /// who is the leader of the campus.</param>
        /// <param name="serviceTimes">A <see cref="System.String"/> representing a delimited list of service times for the campus.</param>
        /// <param name="foreignId">A <see cref="System.String"/> representing the identifier of the Campus in the foreign system that it was imported from.</param>
        /// <param name="campusId">A nullable <see cref="System.Int32"/> representing the internal Campus Identifier (primary key) of the campus. This will allow for the support of updates.</param>
        /// <returns>A nullable <see cref="System.Int32"/> representing the Id of the campus that was either added or updated. Will be null if an update is attempted and the campus was not found. </returns>
        public int? SaveCampus(bool isSystem, string name, string shortCode = null, int? locationId = null, string phoneNumber = null,  int? leaderPersonAliasId = null, string serviceTimes = null, string foreignId = null, int? campusId = null )
        {
            Campus c = null;

            CampusController controller = new CampusController( Service );

            if ( campusId == null || campusId <= 0 )
            {
                c = new Campus();
            }
            else
            {
                c = controller.GetById( (int)campusId );
            }

            // Update was attempted and campus was not found in Arena instance.
            if ( c == null )
            {
                return null;
            }

            c.IsSystem = isSystem;
            c.Name = name;
            c.ShortCode = shortCode;
            c.LocationId = locationId;
            c.LeaderPersonAliasId = leaderPersonAliasId;
            c.PhoneNumber = phoneNumber;
            c.ServiceTimes = serviceTimes;
            c.ForeignId = foreignId;
            c.IsActive = true;

            int? personAliasId = Service.GetCurrentPersonAliasId();
            if ( c.Id > 0 )
            {
                c.ModifiedByPersonAliasId = personAliasId;
                controller.Update(c);
            }
            else
            {
                c.CreatedByPersonAliasId = personAliasId;
                controller.Add( c );
            }

            c = controller.GetByGuid( c.Guid );

            GetCampuses( true );
            return c.Id;
        }

        /// <summary>
        /// Gets Campus by an external system's foreign key value.
        /// </summary>
        /// <param name="foreignId"> A <see cref="System.String"/> representing the foreign key value to find the Campus by.</param>
        /// <returns> A <see cref="System.Collections.Generic.Dictionary(String,Object)"/> representing the properties of the Campus. The key value represents the property name
        /// and the value represents the property value.</returns>
        public Dictionary<string, object> GetByForeignId( string foreignId )
        {
            Dictionary<string, object> selectedCampus = null;

            selectedCampus = GetCampuses().FirstOrDefault( c => c.Value["ForeignId"] != null && c.Value["ForeignId"].ToString() == foreignId ).Value;

            //foreach ( var c in GetCampuses().Select(c => c.Value) )
            //{
            //    if ( c.ContainsKey( "ForeignId" ) && c["ForeignId"] != null && c["ForeignId"].ToString() == foreignId )
            //    {
            //        selectedCampus = c;
            //    }
            //}
                                                    

            if ( selectedCampus == null  )
            {
                //foreach ( var c in GetCampuses(true).Select(c => c.Value) )
                //{
                //    if ( c.ContainsKey( "ForeignId" ) && c["ForeignId"] != null && c["ForeignId"].ToString() == foreignId )
                //    {
                //        selectedCampus = c;
                //    }
                //}

                selectedCampus = GetCampuses( true ).FirstOrDefault( c => c.Value["ForeignId"] != null && c.Value["ForeignId"].ToString() == foreignId ).Value;
            }

            return selectedCampus;
        }

        /// <summary>
        /// Gets campus by Id
        /// </summary>
        /// <param name="id">A <see cref="System.Int32"/> representing the Id value of the <see cref="Rock.Model.Campus"/> to search by.</param>
        /// <returns> A <see cref="System.Collections.Generic.Dictionary(String,Object)"/> representing the properties of the Campus. The key value represents the property name
        /// and the value represents the property value.</returns>
        public Dictionary<string, object> GetById( int id )
        {
            Dictionary<string, object> campus = GetCampuses().Where( c => c.Key == id ).Select( c => c.Value ).FirstOrDefault();

            if( campus == null )
            {
                campus = GetCampuses( true ).Where( c => c.Key == id ).Select( c => c.Value ).FirstOrDefault();
            }

            return campus;
        }


        /// <summary>
        /// Gets a dictionary that contains all the Campuses from RockRMS.  The dictionary will be cached in memory for up to 5 minutes.
        /// </summary>
        /// <param name="resetCache">A <see cref="System.Boolean"/> that indicates if the Campus Cache should be reset. Defaults to false.</param>
        /// <returns>
        /// A <see cref="System.Collections.Generic.Dictionary(int,object)" /> that contains all the campuses in Rock. The key value is the Rock instance Id of the Campus
        /// and the value is a <see cref="System.Collections.Generic.Dictionary(String,Object)" /> containing the entity The key value represents a property name and the
        /// value represents the value of the entity.
        /// </returns>
        public Dictionary<int, Dictionary<string, object>> GetCampuses(bool resetCache = false)
        {
            const string campusCacheKey = "CampusCache";
            Dictionary<int, Dictionary<string, object>> campuses = null;

            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes( 5 );

            if ( !resetCache )
            {
                campuses = ( cache[campusCacheKey] as Dictionary<int, Dictionary<string, object>> );
            }

            if ( resetCache || campuses == null )
            {
                campuses = new Dictionary<int, Dictionary<string, object>>();

                CampusController controller = new CampusController( Service );
                List<Campus> campusList = controller.GetAll();

                foreach ( var campus in campusList )
                {
                    campuses.Add( campus.Id, ToDictionary( campus ) );
                }

                cache.Set( campusCacheKey, campuses, policy );
            }

            return campuses;
        }



        #region Private Methods
        /// <summary>
        /// Returns the Campus entity as a dictionary. This extends the model's ToDictionary method
        /// by including the Id and Guid properties.
        /// </summary>
        /// <param name="c">The Campus.</param>
        /// <returns>A dictionary containing the campus.</returns>
        private Dictionary<string, object> ToDictionary( Campus c )
        {
            Dictionary<string,object> entityDictionary = null;
            if(c != null)
            {
                entityDictionary = c.ToDictionary();

                if ( !entityDictionary.ContainsKey( "CreatedByPersonAliasId" ) )
                {
                    entityDictionary.Add( "CreatedByPersonAliasId", c.CreatedByPersonAliasId );
                }

                if ( !entityDictionary.ContainsKey( "ModifiedByPersonAliasId" ) )
                {
                    entityDictionary.Add( "ModifiedByPersonAliasId", c.ModifiedByPersonAliasId );
                }
                
                if(!entityDictionary.ContainsKey("CreatedDateTime"))
                {
                    entityDictionary.Add( "CreatedDateTime", c.CreatedDateTime );
                }

                if ( !entityDictionary.ContainsKey( "ModifiedDateTime" ) )
                {
                    entityDictionary.Add( "ModifiedDateTime", c.ModifiedDateTime );
                }

            }

            return entityDictionary;
        }
        #endregion



    }
}
