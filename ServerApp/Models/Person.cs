using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServerApp.Models
{
    /// <summary>
    /// Person Class (Gets implemented by Patient and Provider Classes)
    /// </summary>
    public abstract class Person
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        [XmlIgnore]
        public DateTimeOffset CreationTime { get; set; }
        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(128)]
        public string LastName { get; set; }

        /// <summary>
        /// Constructor created new GuidId
        /// </summary>
        public Person()
        {
            Id = Guid.NewGuid();
        }

        [JsonIgnore]
        [NotMapped]
        [XmlElement("CreationTime")]
        public string CreationTimeXML
        {
            get
            {
                return CreationTime.ToString("o");
            }
            set
            {
                CreationTime = DateTimeOffset.Parse(value);
            }
        }
    }
}
