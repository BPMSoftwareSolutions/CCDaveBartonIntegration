using DaveBartonCCIntegrationAPI.DataAccess;
using DaveBartonCCIntegrationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DaveBartonCCIntegrationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EligibilityRecordsController : ControllerBase
    {
        private readonly ServiceScope _scope;

        public EligibilityRecordsController(ServiceScope scope)
        {
            _scope = scope;
        }

        // POST api/eligibilityrecords
        [HttpPost]
        public async Task<IActionResult> SaveEligibilityRecord([FromBody] EligibilityRecord record)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _scope.Repository.SaveEligibilityRecord(record);

                return Ok(new { message = "Eligibility record saved successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception as necessary
                return StatusCode(500, new { message = $"An error occurred while saving the record: {ex.Message}" });
            }
        }


        // GET api/eligibilityrecords
        [HttpGet]
        public async Task<IActionResult> GetEligibilityRecords()
        {
            try
            {
                var records = _scope.Repository.GetAllEligibilityRecords();

                if (records == null)
                {
                    return NotFound(new { message = "No eligibility records found." });
                }

                return Ok(records);
            }
            catch (Exception ex)
            {
                // Log the exception as necessary
                return StatusCode(500, new { message = $"An error occurred while retrieving the records: {ex.Message}" });
            }
        }
    }
}
