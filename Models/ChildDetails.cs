using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildImmunizationCare_Doctor.Models
{
    public class ChildDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("parentId")]
        public string ParentId { get; set; }

        [BsonElement("age")]
        public int Age { get; set; }

        [BsonElement("vaccinated")]
        public bool Vaccinated { get; set; }

        [BsonElement("vaccinatedByDoctorId")]
        public string vaccinatedByDoctorId { get; set; }

        [BsonElement("vaccinatedByDoctorName")]
        public string vaccinatedByDoctorName { get; set; }
    }
}
