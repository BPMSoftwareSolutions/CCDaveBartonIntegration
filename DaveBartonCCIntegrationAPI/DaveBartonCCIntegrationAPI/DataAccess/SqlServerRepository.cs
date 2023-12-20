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
                    command.Parameters.AddWithValue("@SSN", record.SSN);
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
                    command.Parameters.AddWithValue("@SubscriberNumber", record.SubscriberNumber);
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
                                EligibilityRecordId = reader.GetInt32(reader.GetOrdinal("EligibilityRecordId")),
                                SSN = reader.IsDBNull(reader.GetOrdinal("SSN")) ? null : reader.GetString(reader.GetOrdinal("SSN")),
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                                Gender = reader.GetString(reader.GetOrdinal("Gender")),
                                PlanSponsorName = reader.GetString(reader.GetOrdinal("PlanSponsorName")),
                                SponsorIdentifier = reader.IsDBNull(reader.GetOrdinal("SponsorIdentifier")) ? null : reader.GetString(reader.GetOrdinal("SponsorIdentifier")),
                                InsurerName = reader.GetString(reader.GetOrdinal("InsurerName")),
                                InsurerIdentificationCode = reader.IsDBNull(reader.GetOrdinal("InsurerIdentificationCode")) ? null : reader.GetString(reader.GetOrdinal("InsurerIdentificationCode")),
                                InsuredIndicator = reader.GetString(reader.GetOrdinal("InsuredIndicator")),
                                IndividualRelationshipCode = reader.GetString(reader.GetOrdinal("IndividualRelationshipCode")),
                                MaintenanceTypeCode = reader.GetString(reader.GetOrdinal("MaintenanceTypeCode")),
                                MaintenanceReasonCode = reader.GetString(reader.GetOrdinal("MaintenanceReasonCode")),
                                BenefitStatusCode = reader.GetString(reader.GetOrdinal("BenefitStatusCode")),
                                SubscriberNumber = reader.IsDBNull(reader.GetOrdinal("SubscriberNumber")) ? null : reader.GetString(reader.GetOrdinal("SubscriberNumber")),
                                GroupOrPolicyNumber = reader.IsDBNull(reader.GetOrdinal("GroupOrPolicyNumber")) ? null : reader.GetString(reader.GetOrdinal("GroupOrPolicyNumber")),
                                DepartmentOrAgencyNumber = reader.IsDBNull(reader.GetOrdinal("DepartmentOrAgencyNumber")) ? null : reader.GetString(reader.GetOrdinal("DepartmentOrAgencyNumber")),
                                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                StreetAddress = reader.GetString(reader.GetOrdinal("StreetAddress")),
                                CityName = reader.GetString(reader.GetOrdinal("CityName")),
                                StateOrProvinceCode = reader.GetString(reader.GetOrdinal("StateOrProvinceCode")),
                                PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                                MaritalStatus = reader.IsDBNull(reader.GetOrdinal("MaritalStatus")) ? null : reader.GetString(reader.GetOrdinal("MaritalStatus")),
                                InsuranceLineCode = reader.GetString(reader.GetOrdinal("InsuranceLineCode")),
                                CoverageTier = reader.GetString(reader.GetOrdinal("CoverageTier")),
                                EffectiveDate = reader.GetDateTime(reader.GetOrdinal("EffectiveDate")),
                                PlanCoverageDescription = reader.GetString(reader.GetOrdinal("PlanCoverageDescription")),
                                CoverageMaintenanceCode = reader.GetString(reader.GetOrdinal("CoverageMaintenanceCode")),
                                TransactionSetPurposeCode = reader.GetString(reader.GetOrdinal("TransactionSetPurposeCode"))
                            };

                            records.Add(record);
                        }
                    }
                }
            }

            return records;
        }
    }
}
