namespace DaveBartonCCIntegrationAPI.Models
{
    public class EligibilityRecord
    {
        public int EligibilityRecordId { get; set; }
        public string SSN { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public string PlanSponsorName { get; set; }
        public string SponsorIdentifier { get; set; }
        public string InsurerName { get; set; }
        public string InsurerIdentificationCode { get; set; }
        public string InsuredIndicator { get; set; }
        public string IndividualRelationshipCode { get; set; }
        public string MaintenanceTypeCode { get; set; }
        public string MaintenanceReasonCode { get; set; }
        public string BenefitStatusCode { get; set; }
        public string SubscriberNumber { get; set; }
        public string GroupOrPolicyNumber { get; set; }
        public string DepartmentOrAgencyNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string CityName { get; set; }
        public string StateOrProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string MaritalStatus { get; set; }
        public string InsuranceLineCode { get; set; }
        public string CoverageTier { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string PlanCoverageDescription { get; set; }
        public string CoverageMaintenanceCode { get; set; }
        public string TransactionSetPurposeCode { get; set; }
    }
}
