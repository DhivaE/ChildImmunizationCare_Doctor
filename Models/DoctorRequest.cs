using Newtonsoft.Json;

namespace ChildImmunizationCare_Doctor.Models
{
    public class DoctorRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("availableSlot")]
        public string AvailableSlot { get; set; }
    }
}
