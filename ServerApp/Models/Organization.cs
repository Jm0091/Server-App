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
    /// Organization Class
    /// </summary>
    public class Organization
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [XmlIgnore]
        public DateTimeOffset CreationTime { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// Constructor created new GuidId
        /// </summary>
        public Organization()
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
