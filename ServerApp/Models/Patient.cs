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
    /// Patient Class extends Person
    /// </summary>
    public class Patient : Person
    {
        [Required]
        [XmlIgnore]
        public DateTimeOffset DateOfBirth { get; set; }

        [JsonIgnore]
        [NotMapped]
        [XmlElement("DateOfBirth")]
        public string DateOfBirthXML
        {
            get
            {
                return DateOfBirth.ToString("o");
            }
            set
            {
                DateOfBirth = DateTimeOffset.Parse(value);
            }
        }
    }
}
