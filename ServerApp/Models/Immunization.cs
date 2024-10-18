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
    public class Immunization
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [MaxLength(128)]
        [Required]
        public string OfficialName { get; set; }
        [MaxLength(128)]
        public string TradeName { get; set; }
        [MaxLength(255)]
        [Required]
        public string LotNumber { get; set; }
        [Required]
        [XmlIgnore]
        public DateTimeOffset ExpirationDate { get; set; }
        [Required]
        [XmlIgnore]
        public DateTimeOffset CreationTime { get; set; }
        [XmlIgnore]
        public DateTimeOffset UpdatedTime { get; set; }
        /// <summary>
        /// Constructor created new GuidId
        /// </summary>
        public Immunization()
        {
            Id = Guid.NewGuid();
        }

        [JsonIgnore]
        [NotMapped]
        [XmlElement("ExpirationDate")]
        public string ExpirationDateXML
        {
            get
            {
                return ExpirationDate.ToString("o");
            }
            set
            {
                ExpirationDate = DateTimeOffset.Parse(value);
            }
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

        [JsonIgnore]
        [NotMapped]
        [XmlElement("UpdatedTime")]
        public string UpdatedTimeXML
        {
            get
            {
                return UpdatedTime.ToString("o");
            }
            set
            {
                UpdatedTime = DateTimeOffset.Parse(value);
            }
        }
    }
}
