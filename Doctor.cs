using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using MongoDB.Driver;
using ChildImmunizationCare_Doctor.Models;
using System.Linq;

namespace DoctorImmunizationCare_Doctor
{
    public class Doctor
    {
        private readonly IMongoCollection<DoctorDetails> _mongoDoctors;
        private readonly IMongoCollection<ChildDetails> _mongoChild;

        public Doctor()
        {
            var dbClient = new MongoClient("mongodb+srv://dhivakar:dhivakar@cluster0.rmwyf47.mongodb.net/cluster0");
            IMongoDatabase db = dbClient.GetDatabase("ChildImmunizationCare");
            _mongoDoctors = db.GetCollection<DoctorDetails>("Doctor");
            _mongoChild = db.GetCollection<ChildDetails>("Child");
        }


        [FunctionName("InsertDoctor")]
        public async Task<IActionResult> InsertDoctor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Doctors")]
            DoctorRequest doctorRequest
          )
        {

            var doctorToInsert = new DoctorDetails
            {
                Name = doctorRequest.Name,
               AvailableSlot = doctorRequest.AvailableSlot
            };

            await _mongoDoctors.InsertOneAsync(doctorToInsert);

            return new OkResult();
        }

        [FunctionName("UpdateDoctor")]
        public async Task<IActionResult> UpdateDoctor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Doctors/{doctorId}")]
            DoctorRequest doctorRequest,
            HttpRequest req,
            string doctorId,
            ILogger log)
        {

            var fields = new Dictionary<string, Object>();

            if (doctorRequest.Name is not null)
                fields.Add(nameof(DoctorDetails.Name), doctorRequest.Name);


            if (doctorRequest.AvailableSlot is not null)
                fields.Add(nameof(DoctorDetails.AvailableSlot), doctorRequest.AvailableSlot);

            var filter = Builders<DoctorDetails>.Filter.Eq(e => e.Id, doctorId);
            var updates = fields.Select(f => Builders<DoctorDetails>.Update.Set(f.Key, f.Value));
            var update = Builders<DoctorDetails>.Update.Combine(updates);
            var result = await _mongoDoctors.UpdateOneAsync(filter, update);

            return new OkObjectResult(result);
        }

        [FunctionName("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Doctors")]
            HttpRequest req,
           ILogger log)
        {

            var cursor = await _mongoDoctors.FindAsync(Builders<DoctorDetails>.Filter.Empty);
            var list = await cursor.ToListAsync();

            return new OkObjectResult(list);
        }


        [FunctionName("DeleteDoctor")]
        public async Task<IActionResult> DeleteDoctor(
         [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "Doctors/{doctorId}")]
         HttpRequest req,
         string doctorId,
         ILogger log)
        {
            var filter = Builders<DoctorDetails>.Filter
                        .Eq(r => r.Id, doctorId);

            await _mongoDoctors.DeleteOneAsync(filter);

            return new OkResult();
        }


        [FunctionName("DoctorSignOff")]
        public async Task<IActionResult> DoctorSignOff(
        [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "DoctorSignOff/{doctorId}/{childId}")]
         HttpRequest req,
        string doctorId,
        string childId,
        ILogger log)
        {
            var filterDoctor = Builders<DoctorDetails>.Filter
                        .Eq(r => r.Id, doctorId);
            var cursor = await _mongoDoctors.FindAsync(filterDoctor);
            var doctorDetails = await cursor.FirstAsync();

            var fields = new Dictionary<string, Object>
            {
                { nameof(ChildDetails.Vaccinated), true },
                { nameof(ChildDetails.vaccinatedByDoctorId), doctorDetails.Id },
                { nameof(ChildDetails.vaccinatedByDoctorName), doctorDetails.Name }
            };

            var filter = Builders<ChildDetails>.Filter.Eq(e => e.Id, childId);
            var updates = fields.Select(f => Builders<ChildDetails>.Update.Set(f.Key, f.Value));
            var update = Builders<ChildDetails>.Update.Combine(updates);
            var result = await _mongoChild.UpdateOneAsync(filter, update);

            return new OkObjectResult(result);
        }



    }
}
