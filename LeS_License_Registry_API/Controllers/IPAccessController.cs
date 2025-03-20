using LeS_License_Registry_API.Data;
using LeS_License_Registry_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Net;
using System.IdentityModel.Tokens.Jwt;

namespace LeS_License_Registry_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IPAccessController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly ILogger<IPAccessController> _logger;
        private readonly LesLicenseRegistryContext _dataAccess;
        public IPAccessController(TokenService tokenService, ILogger<IPAccessController> logger, LesLicenseRegistryContext dataAccess)
        {
            _tokenService = tokenService;
            _logger = logger;
            _dataAccess = dataAccess;
        }
        [HttpGet]
        [Route("GetIPAccessRecords")]
        public IActionResult GetIPAccessRecords()
        {

            API_Response response = new API_Response();
            response.isSuccess = false;
            try
            {
                var allActivities = _dataAccess.les_ip_access_control.OrderBy(x=>x.ipaccessid).ToList();
                response.totalRecords = allActivities.Count;
                response.isSuccess = true;
                response.data = allActivities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading GetIPAccessRecords!");
                response.isSuccess = false;
                response.message = "Internal server error!";
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("AddUpdateIPaccessRecord")]
        public IActionResult AddUpdateIPaccessRecord([FromBody] LesIpAccessControl modal)
        {
            API_Response response = new API_Response();
            try
            {
                string Username = GetUsernameFromRequest(Request);
                if (modal != null)
                {
                    if (modal.ipaccessid > 0)//update part
                    {

                        var existingModal = _dataAccess.les_ip_access_control.Find(modal.ipaccessid);
                        if (existingModal != null)
                        {
                            if(modal.ip_address!=null&&modal.ip_address.Length>0&& IsValidIPAddress(modal.ip_address))
                            {
                                
                                if (modal.access_type !=null&&( modal.access_type.ToLower() == "allow" || modal.access_type.ToLower() == "block"))
                                {
                                    
                                    if(modal.remarks!=null&& modal.remarks.Length > 0)
                                    {
                                        var logs = GetIPaccessChanges(existingModal, modal);
                                        existingModal.ip_address = modal.ip_address;
                                        existingModal.updated_date = DateTime.Now;
                                        existingModal.access_type = modal.access_type;
                                        existingModal.remarks = modal.remarks;

                                        _dataAccess.les_ip_access_control.Update(existingModal);
                                        int updates = _dataAccess.SaveChanges();
                                        if (updates > 0)
                                        {
                                            if (logs != "")
                                            {
                                                UpdateLogDetails(existingModal.ip_address, Username, logs);
                                            }
                                            response.message = "IP Access record successfully updated!";
                                            response.isSuccess = true;
                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.message = "Something went wrong, Please contact site developer!";
                                        }
                                    }
                                    else
                                    {
                                        response.message = "Please enter valid the remarks!";
                                        response.isSuccess = false;
                                    }
                                   
                                }
                                else
                                {
                                    response.isSuccess = false;
                                    response.message = "Please select valid access type";
                                }
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.message = "Please enter valid IP address!";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.message = "No IP access record found!";
                        }
                    }
                    else//Add part
                    {
                        if (modal.ip_address != null && modal.ip_address.Length > 0 && IsValidIPAddress(modal.ip_address))
                        {
                            var existing = _dataAccess.les_ip_access_control.Any(x=>x.ip_address == modal.ip_address);
                            if (!existing)
                            {


                                if (modal.access_type != null && (modal.access_type.ToLower() == "allow" || modal.access_type.ToLower() == "block"))
                                {
                                    if (!string.IsNullOrEmpty(modal.remarks)&&!string.IsNullOrWhiteSpace(modal.remarks))
                                    {
                                        modal.created_date = DateTime.Now;
                                        _dataAccess.les_ip_access_control.Add(modal);
                                        var logs = GetIPaccessChanges(null, modal);
                                        int updates = _dataAccess.SaveChanges();
                                        if (updates > 0)
                                        {
                                            UpdateLogDetails(modal.ip_address, Username, logs);
                                            response.message = "IP Access record successfully updated!";
                                            response.isSuccess = true;
                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.message = "Something went wrong, Please contact site developer!";
                                        }
                                    }
                                    else
                                    {
                                        response.isSuccess = false;
                                        response.message = "Please enter valid remarks!";
                                    }

                                }
                                else
                                {
                                    response.isSuccess = false;
                                    response.message = "Please select valid access type";
                                }
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.message = "IP address already exist!";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.message = "Please enter valid IP address!";
                        }

                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.message = "Internal server error!";
                }
            }
            catch( Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading AddUpdateIPaccessRecord!");
                response.isSuccess = false;
                response.message = "Internal server error!";
            }
            return Ok(response);
        }
        private bool IsValidIPAddress(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            if (IPAddress.TryParse(ipAddress, out IPAddress? address))
            {
                return address.AddressFamily == AddressFamily.InterNetwork ||
                       address.AddressFamily == AddressFamily.InterNetworkV6;
            }

            return false;
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
                return usernameClaim?.Value ?? "";
            }
            catch (Exception ex)
            {
                // Handle any exceptions (invalid token, parsing errors, etc.)
                throw new Exception("Error parsing the JWT token.", ex);
            }
        }
        public int UpdateLogDetails(string ipaddress, string user, string remarks)
        {
            remarks = remarks.Replace(",", "\r\n");
            LesLicenseUpdateLog modal = new LesLicenseUpdateLog();
            modal.ipaddress = ipaddress;
            modal.updated_date = DateTime.Now;
            modal.updated_by = user;
            modal.remarks = remarks;
            _dataAccess.les_license_update_log.Add(modal);
            return _dataAccess.SaveChanges();
        }
        public string GetUsernameFromRequest(HttpRequest request)
        {
            string username = "";
            try
            {
                string token = GetBearerToken(Request);
                username = GetUsernameFromJwtToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading GetUsernameFromRequest!");
            }
            return username;
        }
        public static string GetIPaccessChanges(LesIpAccessControl? existing, LesIpAccessControl latest)
        {
            if (latest == null)
                return "Invalid input objects.";

            List<string> changes = new List<string>();

            if (existing == null)
            {
                return $"New IP address added as IP Address = '{latest.ip_address}', Status = '{latest.access_type}', and Remarks = '{latest.remarks}'";
            }

            if (existing.ip_address != latest.ip_address)
                changes.Add($"IP Address changed from '{existing.ip_address}' to '{latest.ip_address}' \n\r");

            if (existing.access_type != latest.access_type)
                changes.Add($"Access Type changed from '{existing.access_type}' to '{latest.access_type}' \n\r");

            if (existing.remarks != latest.remarks)
                changes.Add($"Remarks changed from '{existing.remarks}' to '{latest.remarks}' \n\r");

            return changes.Count > 0 ? string.Join(", ", changes) : "No changes detected.";
        }




    }
}
