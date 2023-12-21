// See https://aka.ms/new-console-template for more information
using AirtableApiClient;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using NS = Newtonsoft.Json;
using System.Xml;
using System.Security.Cryptography;

namespace TestDaveBartonAPI
{
    public class Program
    {
        //private static readonly string apiDomain = "davebartonccintegrationapi.azurewebsites.net";
        private static readonly string apiDomain = "localhost:7081";

        static readonly HttpClient client = new HttpClient();
        private static string _token;

        public class AuthToken
        {
            public string token { get; set; }
        }

        static async Task Main(string[] args)
        {
            //_token = await AuthenticateUser($"https://{apiDomain}/api/auth/authenticate", "admin", "SallySav123!");
            //await PullDataFromAirtable();
        }

        static async Task<string> AuthenticateUser(string authUrl, string username, string password)
        {
            var requestBody = new
            {
                Username = username,
                Password = password
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(authUrl, content);

            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStringAsync();
            var responseData = System.Text.Json.JsonSerializer.Deserialize<AuthToken>(responseStream);
            return responseData?.token;
        }

        static async Task<string> CallProtectedApi(string url, string token)
        {
            using (var requestClient = new HttpClient())
            {
                requestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await requestClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task PullDataFromAirtable()
        {
            string offset = null;
            string errorMessage = null;
            var records = new List<AirtableRecord>();

            using (AirtableBase airtableBase = new AirtableBase(Properties.Resources.AirTableAppKey, Properties.Resources.AirTableBaseId))
            {
                do
                {
                    var task = airtableBase.ListRecords(Properties.Resources.AirTableTableId, offset);
                    var response = await task;

                    if (response.Success)
                    {
                        records.AddRange(response.Records.ToList());
                        offset = response.Offset;
                    }
                    else
                    {
                        errorMessage = ExtractErrorMessage(response);
                        break;
                    }
                } while (offset != null);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                // Error reporting
                Console.WriteLine(errorMessage);
            }
            else
            {
                foreach (var record in records)
                {
                    await SendRecordToDatabase(record);
                }
            }
        }

        private static string ExtractErrorMessage(AirtableListRecordsResponse response)
        {
            if (response.AirtableApiError is AirtableApiException apiError)
            {
                string errorMessage = apiError.ErrorMessage;
                if (apiError is AirtableInvalidRequestException)
                {
                    errorMessage += "\nDetailed error message: " + apiError.DetailedErrorMessage;
                }
                return errorMessage;
            }
            return "Unknown error";
        }

        private static async Task SendRecordToDatabase(AirtableRecord record)
        {
            using (var httpClient = new HttpClient())
            {
                var eligibilityRecord = ConvertToEligibilityRecord(record); // Convert AirtableRecord to EligibilityRecord
                var jsonContent = new StringContent(JsonSerializer.Serialize(eligibilityRecord), Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                string endpoint = $"https://{apiDomain}/api/eligibilityrecords";
                var response = await httpClient.PostAsync(endpoint, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    // Handle unsuccessful response
                    Console.WriteLine($"Error sending record to database: {response.StatusCode}");
                    Console.WriteLine($"{ConvertEligibilityRecordToJson(eligibilityRecord)}");

                }
            }
        }

        public static string ConvertEligibilityRecordToJson(EligibilityRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record), "EligibilityRecord cannot be null.");
            }

            string jsonString = NS.JsonConvert.SerializeObject(record, NS.Formatting.Indented);
            return jsonString;
        }

        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(Properties.Resources.AESKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private static EligibilityRecord ConvertToEligibilityRecord(AirtableRecord record)
        {
            return new EligibilityRecord
            {
                SSN = record.Fields.ContainsKey("SSN") ? EncryptString(record.Fields["SSN"]?.ToString()) : string.Empty,
                DateOfBirth = record.Fields.ContainsKey("Date of Birth") ? DateTime.Parse(record.Fields["Date of Birth"]?.ToString()) : (DateTime?)null,
                FullName = record.Fields.ContainsKey("Full Name") ? record.Fields["Full Name"]?.ToString() : string.Empty,
                FirstName = record.Fields.ContainsKey("First Name") ? record.Fields["First Name"]?.ToString() : string.Empty,
                LastName = record.Fields.ContainsKey("Last Name") ? record.Fields["Last Name"]?.ToString() : string.Empty,
                MiddleName = record.Fields.ContainsKey("Middle Name") ? record.Fields["Middle Name"]?.ToString() : string.Empty,
                Gender = record.Fields.ContainsKey("Gender") ? record.Fields["Gender"]?.ToString() : string.Empty,
                PlanSponsorName = record.Fields.ContainsKey("Plan Sponsor Name") ? record.Fields["Plan Sponsor Name"]?.ToString() : string.Empty,
                SponsorIdentifier = record.Fields.ContainsKey("Sponsor Identifier") ? record.Fields["Sponsor Identifier"]?.ToString() : string.Empty,
                InsurerName = record.Fields.ContainsKey("Insurer Name") ? record.Fields["Insurer Name"]?.ToString() : string.Empty,
                InsurerIdentificationCode = record.Fields.ContainsKey("Insurer Identification Code") ? record.Fields["Insurer Identification Code"]?.ToString() : string.Empty,
                InsuredIndicator = record.Fields.ContainsKey("Insured Indicator") ? record.Fields["Insured Indicator"]?.ToString() : string.Empty,
                IndividualRelationshipCode = record.Fields.ContainsKey("Individual Relationship Code") ? record.Fields["Individual Relationship Code"]?.ToString() : string.Empty,
                MaintenanceTypeCode = record.Fields.ContainsKey("Maintenance Type Code") ? record.Fields["Maintenance Type Code"]?.ToString() : string.Empty,
                MaintenanceReasonCode = record.Fields.ContainsKey("Maintenance Reason Code") ? record.Fields["Maintenance Reason Code"]?.ToString() : string.Empty,
                BenefitStatusCode = record.Fields.ContainsKey("Benefit Status Code") ? record.Fields["Benefit Status Code"]?.ToString() : string.Empty,
                SubscriberNumber = record.Fields.ContainsKey("Subscriber Number") ? EncryptString(record.Fields["Subscriber Number"]?.ToString()) : string.Empty,
                GroupOrPolicyNumber = record.Fields.ContainsKey("Group or Policy Number") ? record.Fields["Group or Policy Number"]?.ToString() : string.Empty,
                DepartmentOrAgencyNumber = record.Fields.ContainsKey("Department or agency Number") ? record.Fields["Department or agency Number"]?.ToString() : string.Empty,
                PhoneNumber = record.Fields.ContainsKey("Phone Number") ? record.Fields["Phone Number"]?.ToString() : string.Empty,
                StreetAddress = record.Fields.ContainsKey("Street Address") ? record.Fields["Street Address"]?.ToString() : string.Empty,
                CityName = record.Fields.ContainsKey("City Name") ? record.Fields["City Name"]?.ToString() : string.Empty,
                StateOrProvinceCode = record.Fields.ContainsKey("State or Province Code") ? record.Fields["State or Province Code"]?.ToString() : string.Empty,
                PostalCode = record.Fields.ContainsKey("Postal Code") ? record.Fields["Postal Code"]?.ToString() : string.Empty,
                MaritalStatus = record.Fields.ContainsKey("Marital Status") ? record.Fields["Marital Status"]?.ToString() : string.Empty,
                InsuranceLineCode = record.Fields.ContainsKey("Insurance Line Code") ? record.Fields["Insurance Line Code"]?.ToString() : string.Empty,
                CoverageTier = record.Fields.ContainsKey("Coverage Tier") ? record.Fields["Coverage Tier"]?.ToString() : string.Empty,
                EffectiveDate = record.Fields.ContainsKey("Effective Date") ? DateTime.Parse(record.Fields["Effective Date"]?.ToString()) : (DateTime?)null,
                PlanCoverageDescription = record.Fields.ContainsKey("Plan Coverage Description") ? record.Fields["Plan Coverage Description"]?.ToString() : string.Empty,
                CoverageMaintenanceCode = record.Fields.ContainsKey("Coverage Maintenance Code") ? record.Fields["Coverage Maintenance Code"]?.ToString() : string.Empty,
                TransactionSetPurposeCode = record.Fields.ContainsKey("Transaction Set Purpose Code") ? record.Fields["Transaction Set Purpose Code"]?.ToString() : string.Empty,
                // Add other fields as necessary
            };
        }
    }
}

