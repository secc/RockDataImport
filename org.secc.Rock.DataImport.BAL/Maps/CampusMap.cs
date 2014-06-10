using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Maps
{
    public class CampusMap
    {
        RockService Service { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="CampusMap"/> class from being created.
        /// </summary>
        private CampusMap()
        { }

        public CampusMap(RockService service)
        {
            Service = service;

        }

        /// <summary>
        /// Adds a Campus to the Rock instance.
        /// </summary>
        /// <param name="isSystem">A <see cref="System.Boolean"/> value that is <c>true<c> if the Campus is a system generated value.</param>
        /// <param name="name">A <see cref="System.String"/> representing the unique name of the campus..</param>
        /// <param name="shortCode">A <see cref="System.String"/> representing the optional short code for the campus. Defaults to null.</param>
        /// <param name="locationId">A nullable <see cref="System.Int32"/> that represents the LcoationId  of the <see cref="Rock.Model.Location"/> that is associated with the campus. Defaults to null.</param>
        /// <param name="phoneNumber">A <see cref="System.String"/> representing the phone number of the campus. Defaults to null.</param>
        /// <param name="leaderPersonAliasId">A <see cref="System.Int32"/> that represents the PersonAliasId of a <see cref="Rock.Model.PersonAlias"/> that is associated with the <see cref="Rock.Model.Person"/>
        /// who is the leader of the campus.</param>
        /// <param name="serviceTimes">A <see cref="System.String"/> representing a delimited list of service times for the campus.</param>
        /// <param name="foreignKey">A <see cref="System.String"/> representing the identifier of the Campus in the foreign system that it was imported from.</param>
        /// <returns></returns>
        public int AddCampus(bool isSystem, string name, string shortCode = null, int? locationId = null, string phoneNumber = null,  int? leaderPersonAliasId = null, string serviceTimes = null, string foreignKey = null  )
        {
            Campus c = new Campus();
            c.IsSystem = isSystem;
            c.Name = name;
            c.ShortCode = shortCode;
            c.LocationId = locationId;
            c.LeaderPersonAliasId = leaderPersonAliasId;
            c.PhoneNumber = phoneNumber;
            c.ServiceTimes = serviceTimes;
            c.CreatedByPersonAliasId = Service.LoggedInPerson.Aliases.First().Id;
            c.ForeignId = foreignKey;

            CampusController controller = new CampusController( Service );
            controller.Add( c );

            c = controller.GetByGuid( c.Guid );

            return c.Id;
        }

        /// <summary>
        /// Gets Campus by an external system's foreign key value.
        /// </summary>
        /// <param name="foreignKey"> A <see cref="System.String"/> representing the foreign key value to find the Campus by.</param>
        /// <returns> A <see cref="System.Collections.Generic.Dictionary(String,Object)"/> representing the properties of the Campus. The key value represents the property name
        /// and the value represents the property value.</returns>
        public Dictionary<string, object> GetByForeignKey( string foreignKey )
        {
            CampusController controller = new CampusController( Service );
            Campus campus = controller.GetByForeignKey( foreignKey );

            return ToDictionary( campus );
        }

        /// <summary>
        /// Gets campus by Id
        /// </summary>
        /// <param name="id">A <see cref="System.Int32"/> representing the Id value of the <see cref="Rock.Model.Campus"/> to search by.</param>
        /// <returns> A <see cref="System.Collections.Generic.Dictionary(String,Object)"/> representing the properties of the Campus. The key value represents the property name
        /// and the value represents the property value.</returns>
        public Dictionary<string, object> GetById( int id )
        {
            CampusController controller = new CampusController( Service );
            Campus campus = controller.GetById( id );

            return ToDictionary( campus );
        }

        /// <summary>
        /// Gets a dictionary that contains all the Campuses in Rock RMS.
        /// </summary>
        /// <returns>A <see cref="System.Collections.Generic.Dictionary(int,object)"/> that contains all the campuses in Rock. The key value is the Rock instance Id of the Campus
        /// and the value is a <see cref="System.Collections.Generic.Dictionary(String,Object)"/> containing the entity The key value represents a property name and the 
        /// value represents the value of the entity.</returns>
        public Dictionary<int, Dictionary<string, object>> GetCampuses()
        {
            CampusController controller = new CampusController( Service );
            List<Campus> campusList = controller.GetAll();

            Dictionary<int, Dictionary<string, object>> campusDictionary = new Dictionary<int, Dictionary<string, object>>();

            foreach ( var campus in campusList )
            {
                campusDictionary.Add( campus.Id, ToDictionary( campus ) );
            }

            return campusDictionary;
        }

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
                entityDictionary.Add( "Id", c.Id );
                entityDictionary.Add( "Guid", c.Guid );
                
            }

            return entityDictionary;
        }


    }
}
