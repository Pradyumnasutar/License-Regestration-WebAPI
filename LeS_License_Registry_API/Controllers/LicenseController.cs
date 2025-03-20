using Azure;
using LeS_License_Registry_API.Data;
using LeS_License_Registry_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;

namespace LeS_License_Registry_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
   
    public class LicenseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LicenseController> _logger;
        private readonly LesLicenseRegistryContext _dataAccess;
        public LicenseController(IConfiguration configuration, ILogger<LicenseController> logger,LesLicenseRegistryContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _dataAccess = context;
            
        }

        [HttpGet]
        [Route("GetSecureData")]
        public IActionResult GetSecureData()
        {
            return Ok(new { message = "You have accessed a protected endpoint!" });
        }

        [HttpGet]
        [Route("GetAllCustomers")]
        public IActionResult GetAllCustomers()
        {
            API_Response response = new API_Response();
            try
            {
                List<LesLicenseRegistry>? AllCusts = _dataAccess.les_license_registry?.OrderBy(c => c.Licenseid).ToList();
                if (AllCusts == null)
                {
                    _logger.LogError("No customers found in LesLicenseRegistry "); 
                }
                else
                {
                    response .totalRecords = AllCusts.Count;
                    response.isSuccess = true;
                    response.data = AllCusts;
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading customers!");
                response.isSuccess = false;
                response.message = "Internal server error!";
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetCustomerLinkedApplications")]
        public IActionResult GetCustomerLinkedApplications(int licenseid)
        {
            API_Response response = new API_Response();
            response.isSuccess = false;
            try
            {
                if (licenseid > 0)
                {
                    var LinkedApplications = _dataAccess.les_license_applications.Where(x=>x.licenseid == licenseid).OrderBy(x=>x.licenseapplicationid).ToList();  
                    response.totalRecords = LinkedApplications.Count;
                    response.data = LinkedApplications;
                    response.isSuccess = true;
                }
                else
                {
                    response.totalRecords = 0;
                    response.isSuccess = false;
                    response.message = "Please provide valid license details!";
                }
            }
            catch( Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading GetCustomerLinkedApplications!");
                response.isSuccess = false;
                response.message = "Internal server error!";
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetCustomerDetails")]
        public IActionResult GetCustomerDetails(int licenseId)
        {
            API_Response response = new API_Response();
            response.isSuccess = false;
            try
            {
                if (licenseId > 0)
                {
                    CustomerDetails obj = new CustomerDetails();

                    obj.customer = _dataAccess.les_license_registry.Find(licenseId);
                    obj.Applications = _dataAccess.les_license_applications.Where(x => x.licenseid == licenseId).ToList();
                    response.isSuccess = true;
                    response.data = JsonConvert.SerializeObject(obj);

                }
                else
                {
                    response.message = "No customer id found!";
                }

            }
            catch( Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading GetCustomerDetails!");
                response.isSuccess = false;
                response.message = "Internal server error!";

            }
            return Ok(response);
        }

        [HttpPost]
        [Route("UpdateCustomerLicense")]
        public IActionResult UpdateCustomerLicense([FromBody]LicenseUpdateModal modal)
        {
            var DefaultWarnCount = Convert.ToInt32( _configuration["Appsettings:LICENSE_EXPIRY_MAX_WARNINGS"]);
            API_Response response = new API_Response();
            response.isSuccess = false;
            try
            {
                string Username = GetUsernameFromRequest(Request);
                if (modal != null && modal.LicenseId > 0)
                {
                    if (!string.IsNullOrEmpty(modal.Status)&&!string.IsNullOrWhiteSpace(modal.Status))
                    {
                        if (modal.Status.ToLower() == "active" || modal.Status.ToLower() == "revoked" || modal.Status.ToLower() == "expired")
                        {
                            if (modal.ExpiryDate != DateTime.MinValue && modal.ExpiryDate != DateTime.MaxValue&& modal.ExpiryDate >= DateTime.Now)
                            {
                                if (!string.IsNullOrEmpty(modal.Remarks)&&!string.IsNullOrWhiteSpace(modal.Remarks))
                                {
                                    bool defaultchecker = false;
                                    var license = _dataAccess.les_license_registry.Find(modal.LicenseId);
                                    if (license != null)
                                    {
                                        bool revokeddt = false;
                                        if (modal.ExpiryDate > license.expiry_date)
                                        {
                                            defaultchecker = true;
                                            _logger.LogInformation("Warning count set to default count :" + DefaultWarnCount + " for customer : " + license.customer_name);
                                        }

                                        else
                                        {
                                            if (modal.Status.ToLower() == "revoked")
                                            {
                                                revokeddt = true;
                                            }

                                            string Logs = GetLicenseChanges(license, modal);
                                            modal.ExpiryDate = Convert.ToDateTime(modal.ExpiryDate);
                                            license.status = modal.Status;
                                            license.expiry_date = modal.ExpiryDate;
                                            license.remarks = modal.Remarks;
                                            license.warningcount = modal.warningcount;
                                            if (modal.Status.ToLower() == "active")
                                            {
                                                license.activated_date = DateTime.Now;
                                            }
                                            if (revokeddt)
                                            {
                                                license.revoked_date = DateTime.Now;
                                            }
                                            else
                                            {
                                                license.revoked_date = null;
                                            }
                                            license.warningdaysbeforeexpiry = modal.warningdays;

                                            if (defaultchecker)
                                            {
                                                string log = "Warning count set to default count :" + DefaultWarnCount;
                                                Logs = Logs + ("\n\r" + log);
                                                license.warningcount = DefaultWarnCount;
                                            }
                                            _dataAccess.les_license_registry.Update(license);
                                            int updates = _dataAccess.SaveChanges();
                                            if (Logs != "")
                                            {
                                                UpdateLogDetails(license.status, license.Licenseid, Username, Logs);
                                            }


                                            if (updates > 0)
                                            {

                                                response.isSuccess = true;
                                                response.message = "License details updated successfully!";
                                                response.data = license;
                                            }
                                            else
                                            {
                                                response.message = "Unable to update changes!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        response.message = "No license details found!";
                                    }

                                }
                                else
                                {
                                    response.message = "Please enter valid remarks!";
                                }
                                
                            }
                            else
                            {
                                response.message = "Please enter valid expiry date!";
                            }
                        }
                        else
                        {
                            response.message = "Please enter valid status!";
                        }
                    }
                    else
                    {
                        response.message = "Please enter valid status!";
                    }
                }
                else
                {
                    response.message = "No customer data found!";

                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading UpdateCustomerLicense!");
                response.isSuccess = false;
                response.message = "Internal server error!";
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("SyncLicense")]
        public IActionResult SyncLicense()
        {
            try
            {
                return SyncLicenseCache();
            }
            catch( Exception ex)
            {
                _logger.LogError("Error occurred at SyncLicense: " + ex.GetBaseException());
                return BadRequest("Something went wrong while processing request!");
            }
            
        }

        [HttpPost]
        [Route("UpdateApplicationStatus")]
        public IActionResult UpdateApplicationStatus([FromBody]UpdateApplicationStatusModal modal)
        {
            API_Response response = new API_Response();
            try
            {
                var username = GetUsernameFromRequest(Request);
                if (modal.licenseapplicationid > 0)
                {
                    var Application = _dataAccess.les_license_applications.Find(modal.licenseapplicationid);
                    
                    if(Application!= null)
                    {
                        var License = _dataAccess.les_license_registry.Find(Application.licenseid);
                        if(License!= null)
                        {
                            Application.isactive = modal.isactive;

                            _dataAccess.les_license_applications.Update(Application);
                            int updated = _dataAccess.SaveChanges();
                            if (updated > 0)
                            {
                                string status = "";
                                if (modal.isactive)
                                {
                                    status = "Active";
                                }
                                else
                                {
                                    status = "Disabled";
                                }
                                string log = "Application " + Application.application_name + " status is changed to : " + status;
                                UpdateLogDetails(License.status, License.Licenseid, username, log);
                                response.isSuccess = true;
                                response.message = "Application status successfully updated !";
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.message = "Unable to updated application status!";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.message = "No License details found!";
                        }
                       
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.message = "No application details found!";
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.message = "No application details found!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading UpdateApplicationStatus!");
                response.isSuccess = false;
                response.message = "Internal server error!";
            }
            return Ok(response);
        }


        private IActionResult SyncLicenseCache()
        {
            try
            {
                var apiUrl = _configuration["Appsettings:SyncLicenseAPIEndPoint"];
                var client = new RestClient();
                var request = new RestRequest(apiUrl, Method.Post);
                RestResponse response = client.Execute(request);

                _logger.LogInformation("SyncLicenseCache API Response: " + response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { status = "Success", response = response.Content });
                }
                else
                {
                    _logger.LogError("Bad request error from Service SyncLicenseAPIEndPoint");
                    return BadRequest(new { status = "Error", message = response.ErrorException?.Message ?? "API request failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected exception in SyncLicenseCache: " + ex.GetBaseException().ToString());
                return BadRequest(new { status = "Error", message = ex.Message });
            }
        }

        private string GetLicenseChanges(LesLicenseRegistry existing, LicenseUpdateModal latest)
        {
            if (existing == null || latest == null)
                return "Invalid input objects.";

            List<string> changes = new List<string>();

            if (existing.status != latest.Status)
                changes.Add($"Status changed from '{existing.status}' to '{latest.Status}'");

            if (existing.remarks != latest.Remarks)
                changes.Add($"Remarks changed from '{existing.remarks}' to '{latest.Remarks}'");

            
            if (existing.warningcount != latest.warningcount)
                changes.Add($"Warning Count changed from '{existing.warningcount}' to '{latest.warningcount}'");

            if (existing.warningdaysbeforeexpiry != latest.warningdays)
                changes.Add($"Warning Days Before Expiry changed from '{existing.warningdaysbeforeexpiry}' to '{latest.warningdays}'");

            if (existing.expiry_date != latest.ExpiryDate)
                changes.Add($"Expiry Date changed from '{existing.expiry_date}' to '{latest.ExpiryDate}'");

            return changes.Count > 0 ? string.Join(", ", changes) : "";
        }

        public int UpdateLogDetails(string? licensestatus,int licid,string user, string remarks)
        {
            remarks = remarks.Replace(",", "\r\n");
            LesLicenseUpdateLog modal = new LesLicenseUpdateLog();
            modal.licenseid = licid;
            modal.updated_date = DateTime.Now;
            modal.updated_by = user;
            modal.remarks = remarks;
            modal.license_status = licensestatus;
            _dataAccess.les_license_update_log.Add(modal);
            return _dataAccess.SaveChanges();
        }
        public string GetBearerToken(HttpRequest request)
        {
            // Check if the Authorization header exists
            if (request.Headers.ContainsKey("Authorization"))
            {
                var authorizationHeader = request.Headers["Authorization"].FirstOrDefault();

                if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
                {
                    // Extract and return the token (remove "Bearer " prefix)
                    return authorizationHeader.Substring("Bearer ".Length).Trim();
                }
            }

            // Return null or throw an exception if token is not found
            return "";
        }
        public string GetUsernameFromJwtToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));
            }

            try
            {
                // Create a JWT token handler
                var handler = new JwtSecurityTokenHandler();

                // Read and validate the JWT token
                var jwtToken = handler.ReadJwtToken(token);

                // Extract the 'username' claim (you can change this to whatever claim you want)
                var usernameClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "sub");

                // Return the username (null if not found)
                return usernameClaim?.Value??"";
            }
            catch (Exception ex)
            {
                // Handle any exceptions (invalid token, parsing errors, etc.)
                throw new Exception("Error parsing the JWT token.", ex);
            }
        }
        public string GetUsernameFromRequest(HttpRequest request)
        {
            string username = "";
            try
            {
                string token = GetBearerToken(Request);
                username = GetUsernameFromJwtToken(token);
            }
            catch( Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading GetUsernameFromRequest!");
            }
            return username;
        }
       
    }
    public class UpdateApplicationStatusModal
    {
        public int licenseapplicationid { get; set; }
        public bool isactive {  get; set; } 
    }

}
