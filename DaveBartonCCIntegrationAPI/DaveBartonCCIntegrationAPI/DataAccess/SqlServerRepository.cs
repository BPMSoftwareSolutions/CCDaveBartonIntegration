using Microsoft.Data.SqlClient;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using DaveBartonCCIntegrationAPI.Models;

namespace DaveBartonCCIntegrationAPI.DataAccess
{
    public class SqlServerRepository
    {
        private readonly string _connectionString;
        private readonly string _aesKey;

        public SqlServerRepository(string connectionString, string aesKey)
        {
            _connectionString = connectionString;
            _aesKey = aesKey;
        }

        private string ComputeHash(string input)
        {
            if (input == null) input = "ResetMe123!";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashedBytes = sha256.ComputeHash(inputBytes);

                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public bool ValidateUser(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT PasswordHash FROM [User] WHERE Username = @username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        string storedPasswordHash = result.ToString();
                        string userPasswordHash = ComputeHash(password);

                        // Validate the hashed password with the input
                        return storedPasswordHash == userPasswordHash;
                    }
                    return false;
                }
            }
        }

        public void SaveEligibilityRecord(EligibilityRecord record)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_InsertEligibilityRecord";

                    // Add parameters to the command from the EligibilityRecord object
                    command.Parameters.AddWithValue("@SSN", DecryptString(record.SSN));
                    command.Parameters.AddWithValue("@DateOfBirth", record.DateOfBirth);
                    command.Parameters.AddWithValue("@FullName", record.FullName);
                    command.Parameters.AddWithValue("@FirstName", record.FirstName);
                    command.Parameters.AddWithValue("@LastName", record.LastName);
                    command.Parameters.AddWithValue("@MiddleName", record.MiddleName);
                    command.Parameters.AddWithValue("@Gender", record.Gender);
                    command.Parameters.AddWithValue("@PlanSponsorName", record.PlanSponsorName);
                    command.Parameters.AddWithValue("@SponsorIdentifier", record.SponsorIdentifier);
                    command.Parameters.AddWithValue("@InsurerName", record.InsurerName);
                    command.Parameters.AddWithValue("@InsurerIdentificationCode", record.InsurerIdentificationCode);
                    command.Parameters.AddWithValue("@InsuredIndicator", record.InsuredIndicator);
                    command.Parameters.AddWithValue("@IndividualRelationshipCode", record.IndividualRelationshipCode);
                    command.Parameters.AddWithValue("@MaintenanceTypeCode", record.MaintenanceTypeCode);
                    command.Parameters.AddWithValue("@MaintenanceReasonCode", record.MaintenanceReasonCode);
                    command.Parameters.AddWithValue("@BenefitStatusCode", record.BenefitStatusCode);
                    command.Parameters.AddWithValue("@SubscriberNumber", DecryptString(record.SubscriberNumber));
                    command.Parameters.AddWithValue("@GroupOrPolicyNumber", record.GroupOrPolicyNumber);
                    command.Parameters.AddWithValue("@DepartmentOrAgencyNumber", record.DepartmentOrAgencyNumber);
                    command.Parameters.AddWithValue("@PhoneNumber", record.PhoneNumber);
                    command.Parameters.AddWithValue("@StreetAddress", record.StreetAddress);
                    command.Parameters.AddWithValue("@CityName", record.CityName);
                    command.Parameters.AddWithValue("@StateOrProvinceCode", record.StateOrProvinceCode);
                    command.Parameters.AddWithValue("@PostalCode", record.PostalCode);
                    command.Parameters.AddWithValue("@MaritalStatus", record.MaritalStatus);
                    command.Parameters.AddWithValue("@InsuranceLineCode", record.InsuranceLineCode);
                    command.Parameters.AddWithValue("@CoverageTier", record.CoverageTier);
                    command.Parameters.AddWithValue("@EffectiveDate", record.EffectiveDate);
                    command.Parameters.AddWithValue("@PlanCoverageDescription", record.PlanCoverageDescription);
                    command.Parameters.AddWithValue("@CoverageMaintenanceCode", record.CoverageMaintenanceCode);
                    command.Parameters.AddWithValue("@TransactionSetPurposeCode", record.TransactionSetPurposeCode);

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<EligibilityRecord> GetAllEligibilityRecords()
        {
            var records = new List<EligibilityRecord>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT * FROM EligibilityRecord", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var record = new EligibilityRecord
                            {
                                EligibilityRecordId = reader.IsDBNull(reader.GetOrdinal("EligibilityRecordId")) ? default(int) : reader.GetInt32(reader.GetOrdinal("EligibilityRecordId")),
                                SSN = reader.IsDBNull(reader.GetOrdinal("SSN")) ? null : EncryptString(reader.GetString(reader.GetOrdinal("SSN"))),
                                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString(reader.GetOrdinal("FullName")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                                Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? null : reader.GetString(reader.GetOrdinal("Gender")),
                                PlanSponsorName = reader.IsDBNull(reader.GetOrdinal("PlanSponsorName")) ? null : reader.GetString(reader.GetOrdinal("PlanSponsorName")),
                                SponsorIdentifier = reader.IsDBNull(reader.GetOrdinal("SponsorIdentifier")) ? null : reader.GetString(reader.GetOrdinal("SponsorIdentifier")),
                                InsurerName = reader.IsDBNull(reader.GetOrdinal("InsurerName")) ? null : reader.GetString(reader.GetOrdinal("InsurerName")),
                                InsurerIdentificationCode = reader.IsDBNull(reader.GetOrdinal("InsurerIdentificationCode")) ? null : reader.GetString(reader.GetOrdinal("InsurerIdentificationCode")),
                                InsuredIndicator = reader.IsDBNull(reader.GetOrdinal("InsuredIndicator")) ? null : reader.GetString(reader.GetOrdinal("InsuredIndicator")),
                                IndividualRelationshipCode = reader.IsDBNull(reader.GetOrdinal("IndividualRelationshipCode")) ? null : reader.GetString(reader.GetOrdinal("IndividualRelationshipCode")),
                                MaintenanceTypeCode = reader.IsDBNull(reader.GetOrdinal("MaintenanceTypeCode")) ? null : reader.GetString(reader.GetOrdinal("MaintenanceTypeCode")),
                                MaintenanceReasonCode = reader.IsDBNull(reader.GetOrdinal("MaintenanceReasonCode")) ? null : reader.GetString(reader.GetOrdinal("MaintenanceReasonCode")),
                                BenefitStatusCode = reader.IsDBNull(reader.GetOrdinal("BenefitStatusCode")) ? null : reader.GetString(reader.GetOrdinal("BenefitStatusCode")),
                                SubscriberNumber = reader.IsDBNull(reader.GetOrdinal("SubscriberNumber")) ? null : EncryptString(reader.GetString(reader.GetOrdinal("SubscriberNumber"))),
                                GroupOrPolicyNumber = reader.IsDBNull(reader.GetOrdinal("GroupOrPolicyNumber")) ? null : reader.GetString(reader.GetOrdinal("GroupOrPolicyNumber")),
                                DepartmentOrAgencyNumber = reader.IsDBNull(reader.GetOrdinal("DepartmentOrAgencyNumber")) ? null : reader.GetString(reader.GetOrdinal("DepartmentOrAgencyNumber")),
                                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                StreetAddress = reader.IsDBNull(reader.GetOrdinal("StreetAddress")) ? null : reader.GetString(reader.GetOrdinal("StreetAddress")),
                                CityName = reader.IsDBNull(reader.GetOrdinal("CityName")) ? null : reader.GetString(reader.GetOrdinal("CityName")),
                                StateOrProvinceCode = reader.IsDBNull(reader.GetOrdinal("StateOrProvinceCode")) ? null : reader.GetString(reader.GetOrdinal("StateOrProvinceCode")),
                                PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader.GetString(reader.GetOrdinal("PostalCode")),
                                MaritalStatus = reader.IsDBNull(reader.GetOrdinal("MaritalStatus")) ? null : reader.GetString(reader.GetOrdinal("MaritalStatus")),
                                InsuranceLineCode = reader.IsDBNull(reader.GetOrdinal("InsuranceLineCode")) ? null : reader.GetString(reader.GetOrdinal("InsuranceLineCode")),
                                CoverageTier = reader.IsDBNull(reader.GetOrdinal("CoverageTier")) ? null : reader.GetString(reader.GetOrdinal("CoverageTier")),
                                EffectiveDate = reader.IsDBNull(reader.GetOrdinal("EffectiveDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EffectiveDate")),
                                PlanCoverageDescription = reader.IsDBNull(reader.GetOrdinal("PlanCoverageDescription")) ? null : reader.GetString(reader.GetOrdinal("PlanCoverageDescription")),
                                CoverageMaintenanceCode = reader.IsDBNull(reader.GetOrdinal("CoverageMaintenanceCode")) ? null : reader.GetString(reader.GetOrdinal("CoverageMaintenanceCode")),
                                TransactionSetPurposeCode = reader.IsDBNull(reader.GetOrdinal("TransactionSetPurposeCode")) ? null : reader.GetString(reader.GetOrdinal("TransactionSetPurposeCode"))
                            };

                            records.Add(record);
                        }
                    }
                }
            }

            return records;
        }

        public string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(_aesKey);
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

        public string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(_aesKey); // Decode the Base64 key string
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
