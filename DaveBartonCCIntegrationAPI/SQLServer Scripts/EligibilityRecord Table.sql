drop table EligibilityRecord
go
CREATE TABLE EligibilityRecord (
	EligibilityRecordId int identity(1,1) primary key,
    SSN NVARCHAR(128),
    DateOfBirth datetime,
    FullName NVARCHAR(50),
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    MiddleName NVARCHAR(20),
    Gender NVARCHAR(10),
    PlanSponsorName NVARCHAR(50),
    SponsorIdentifier NVARCHAR(9),
    InsurerName NVARCHAR(50),
    InsurerIdentificationCode NVARCHAR(9),
    InsuredIndicator NVARCHAR(50),
    IndividualRelationshipCode NVARCHAR(50),
    MaintenanceTypeCode NVARCHAR(50),
    MaintenanceReasonCode NVARCHAR(50),
    BenefitStatusCode NVARCHAR(10),
    SubscriberNumber NVARCHAR(128),
    GroupOrPolicyNumber NVARCHAR(10),
    DepartmentOrAgencyNumber NVARCHAR(50),
    PhoneNumber NVARCHAR(20),
    StreetAddress NVARCHAR(50),
    CityName NVARCHAR(50),
    StateOrProvinceCode NVARCHAR(3),
    PostalCode NVARCHAR(10),
    MaritalStatus NVARCHAR(25),
    InsuranceLineCode NVARCHAR(50),
    CoverageTier NVARCHAR(50),
    EffectiveDate datetime,
    PlanCoverageDescription NVARCHAR(50),
    CoverageMaintenanceCode NVARCHAR(50),
    TransactionSetPurposeCode NVARCHAR(50),
	CreatedDate datetime not null default(getdate())
);
go

alter PROCEDURE sp_InsertEligibilityRecord
    @SSN NVARCHAR(128),
    @DateOfBirth datetime = null,
    @FullName NVARCHAR(50),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @MiddleName NVARCHAR(20),
    @Gender NVARCHAR(10),
    @PlanSponsorName NVARCHAR(50),
    @SponsorIdentifier NVARCHAR(9),
    @InsurerName NVARCHAR(50),
    @InsurerIdentificationCode NVARCHAR(9),
    @InsuredIndicator NVARCHAR(50),
    @IndividualRelationshipCode NVARCHAR(50),
    @MaintenanceTypeCode NVARCHAR(50),
    @MaintenanceReasonCode NVARCHAR(50),
    @BenefitStatusCode NVARCHAR(10),
    @SubscriberNumber NVARCHAR(128),
    @GroupOrPolicyNumber NVARCHAR(10),
    @DepartmentOrAgencyNumber NVARCHAR(50),
    @PhoneNumber NVARCHAR(20),
    @StreetAddress NVARCHAR(50),
    @CityName NVARCHAR(50),
    @StateOrProvinceCode NVARCHAR(3),
    @PostalCode NVARCHAR(10),
    @MaritalStatus NVARCHAR(25),
    @InsuranceLineCode NVARCHAR(50),
    @CoverageTier NVARCHAR(50),
    @EffectiveDate datetime = null,
    @PlanCoverageDescription NVARCHAR(50),
    @CoverageMaintenanceCode NVARCHAR(50),
    @TransactionSetPurposeCode NVARCHAR(50)
AS
BEGIN
    -- Inserting the data into the EligibilityRecord table
    INSERT INTO EligibilityRecord (
        SSN,
        DateOfBirth,
        FullName,
        FirstName,
        LastName,
        MiddleName,
        Gender,
        PlanSponsorName,
        SponsorIdentifier,
        InsurerName,
        InsurerIdentificationCode,
        InsuredIndicator,
        IndividualRelationshipCode,
        MaintenanceTypeCode,
        MaintenanceReasonCode,
        BenefitStatusCode,
        SubscriberNumber,
        GroupOrPolicyNumber,
        DepartmentOrAgencyNumber,
        PhoneNumber,
        StreetAddress,
        CityName,
        StateOrProvinceCode,
        PostalCode,
        MaritalStatus,
        InsuranceLineCode,
        CoverageTier,
        EffectiveDate,
        PlanCoverageDescription,
        CoverageMaintenanceCode,
        TransactionSetPurposeCode
    )
    VALUES (
        @SSN,
        @DateOfBirth,
        @FullName,
        @FirstName,
        @LastName,
        @MiddleName,
        @Gender,
        @PlanSponsorName,
        @SponsorIdentifier,
        @InsurerName,
        @InsurerIdentificationCode,
        @InsuredIndicator,
        @IndividualRelationshipCode,
        @MaintenanceTypeCode,
        @MaintenanceReasonCode,
        @BenefitStatusCode,
        @SubscriberNumber,
        @GroupOrPolicyNumber,
        @DepartmentOrAgencyNumber,
        @PhoneNumber,
        @StreetAddress,
        @CityName,
        @StateOrProvinceCode,
        @PostalCode,
        @MaritalStatus,
        @InsuranceLineCode,
        @CoverageTier,
        @EffectiveDate,
        @PlanCoverageDescription,
        @CoverageMaintenanceCode,
        @TransactionSetPurposeCode
    );
END
GO

--CREATE TABLE [dbo].[User](
--	[UserId] [int] IDENTITY(1,1) NOT NULL,
--	[Username] [nvarchar](255) NOT NULL,
--	[PasswordHash] [nvarchar](255) NOT NULL,
--	[FirstName] [nvarchar](255) NULL,
--	[LastName] [nvarchar](255) NULL,
--	[Email] [nvarchar](255) NULL,
--PRIMARY KEY CLUSTERED 
--(
--	[UserId] ASC
--)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
--UNIQUE NONCLUSTERED 
--(
--	[Username] ASC
--)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--) ON [PRIMARY]
--GO

