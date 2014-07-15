namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddressFields",
                c => new
                    {
                        AddressFieldID = c.Int(nullable: false, identity: true),
                        FieldID = c.Int(nullable: false),
                        AddressString = c.String(),
                        Latitude = c.Decimal(precision: 18, scale: 8),
                        Longitude = c.Decimal(precision: 18, scale: 8),
                        LastGeocode = c.DateTime(),
                        Status = c.Byte(nullable: false),
                        Geo = c.Geography(),
                    })
                .PrimaryKey(t => t.AddressFieldID)
                .ForeignKey("dbo.Fields", t => t.FieldID)
                .Index(t => t.FieldID);
            
            CreateTable(
                "dbo.Fields",
                c => new
                    {
                        FieldID = c.Int(nullable: false, identity: true),
                        TblRowID = c.Int(nullable: false),
                        FieldDefinitionID = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.FieldID)
                .ForeignKey("dbo.FieldDefinitions", t => t.FieldDefinitionID)
                .ForeignKey("dbo.TblRows", t => t.TblRowID)
                .Index(t => t.TblRowID)
                .Index(t => t.FieldDefinitionID);
            
            CreateTable(
                "dbo.ChoiceFields",
                c => new
                    {
                        ChoiceFieldID = c.Int(nullable: false, identity: true),
                        FieldID = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ChoiceFieldID)
                .ForeignKey("dbo.Fields", t => t.FieldID)
                .Index(t => t.FieldID);
            
            CreateTable(
                "dbo.ChoiceInFields",
                c => new
                    {
                        ChoiceInFieldID = c.Int(nullable: false, identity: true),
                        ChoiceFieldID = c.Int(nullable: false),
                        ChoiceInGroupID = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ChoiceInFieldID)
                .ForeignKey("dbo.ChoiceInGroups", t => t.ChoiceInGroupID)
                .ForeignKey("dbo.ChoiceFields", t => t.ChoiceFieldID)
                .Index(t => t.ChoiceFieldID)
                .Index(t => t.ChoiceInGroupID);
            
            CreateTable(
                "dbo.ChoiceInGroups",
                c => new
                    {
                        ChoiceInGroupID = c.Int(nullable: false, identity: true),
                        ChoiceGroupID = c.Int(nullable: false),
                        ChoiceNum = c.Int(nullable: false),
                        ChoiceText = c.String(nullable: false, maxLength: 50),
                        ActiveOnDeterminingGroupChoiceInGroupID = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ChoiceInGroupID)
                .ForeignKey("dbo.ChoiceGroups", t => t.ChoiceGroupID)
                .ForeignKey("dbo.ChoiceInGroups", t => t.ActiveOnDeterminingGroupChoiceInGroupID)
                .Index(t => t.ChoiceGroupID)
                .Index(t => t.ActiveOnDeterminingGroupChoiceInGroupID);
            
            CreateTable(
                "dbo.ChoiceGroups",
                c => new
                    {
                        ChoiceGroupID = c.Int(nullable: false, identity: true),
                        PointsManagerID = c.Int(nullable: false),
                        AllowMultipleSelections = c.Boolean(nullable: false),
                        Alphabetize = c.Boolean(nullable: false),
                        InvisibleWhenEmpty = c.Boolean(nullable: false),
                        ShowTagCloud = c.Boolean(nullable: false),
                        PickViaAutoComplete = c.Boolean(nullable: false),
                        DependentOnChoiceGroupID = c.Int(),
                        ShowAllPossibilitiesIfNoDependentChoice = c.Boolean(nullable: false),
                        AlphabetizeWhenShowingAllPossibilities = c.Boolean(nullable: false),
                        AllowAutoAddWhenAddingFields = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ChoiceGroupID)
                .ForeignKey("dbo.Users", t => t.Creator)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .ForeignKey("dbo.ChoiceGroups", t => t.DependentOnChoiceGroupID)
                .Index(t => t.PointsManagerID)
                .Index(t => t.DependentOnChoiceGroupID)
                .Index(t => t.Creator);
            
            CreateTable(
                "dbo.ChoiceGroupFieldDefinitions",
                c => new
                    {
                        ChoiceGroupFieldDefinitionID = c.Int(nullable: false, identity: true),
                        ChoiceGroupID = c.Int(nullable: false),
                        FieldDefinitionID = c.Int(nullable: false),
                        DependentOnChoiceGroupFieldDefinitionID = c.Int(),
                        TrackTrustBasedOnChoices = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ChoiceGroupFieldDefinitionID)
                .ForeignKey("dbo.ChoiceGroupFieldDefinitions", t => t.DependentOnChoiceGroupFieldDefinitionID)
                .ForeignKey("dbo.FieldDefinitions", t => t.FieldDefinitionID)
                .ForeignKey("dbo.ChoiceGroups", t => t.ChoiceGroupID)
                .Index(t => t.ChoiceGroupID)
                .Index(t => t.FieldDefinitionID)
                .Index(t => t.DependentOnChoiceGroupFieldDefinitionID);
            
            CreateTable(
                "dbo.FieldDefinitions",
                c => new
                    {
                        FieldDefinitionID = c.Int(nullable: false, identity: true),
                        TblID = c.Int(nullable: false),
                        FieldNum = c.Int(nullable: false),
                        FieldName = c.String(nullable: false, maxLength: 50),
                        FieldType = c.Int(nullable: false),
                        UseAsFilter = c.Boolean(nullable: false),
                        AddToSearchWords = c.Boolean(nullable: false),
                        DisplayInTableSettings = c.Int(nullable: false),
                        DisplayInPopupSettings = c.Int(nullable: false),
                        DisplayInTblRowPageSettings = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                        NumNonNull = c.Int(nullable: false),
                        ProportionNonNull = c.Double(nullable: false),
                        UsingNonSparseColumn = c.Boolean(nullable: false),
                        ShouldUseNonSparseColumn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FieldDefinitionID)
                .ForeignKey("dbo.Tbls", t => t.TblID)
                .Index(t => t.TblID);
            
            CreateTable(
                "dbo.DateTimeFieldDefinitions",
                c => new
                    {
                        DateTimeFieldDefinitionID = c.Int(nullable: false, identity: true),
                        FieldDefinitionID = c.Int(nullable: false),
                        IncludeDate = c.Boolean(nullable: false),
                        IncludeTime = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.DateTimeFieldDefinitionID)
                .ForeignKey("dbo.FieldDefinitions", t => t.FieldDefinitionID)
                .Index(t => t.FieldDefinitionID);
            
            CreateTable(
                "dbo.NumberFieldDefinitions",
                c => new
                    {
                        NumberFieldDefinitionID = c.Int(nullable: false, identity: true),
                        FieldDefinitionID = c.Int(nullable: false),
                        Minimum = c.Decimal(precision: 18, scale: 4),
                        Maximum = c.Decimal(precision: 18, scale: 4),
                        DecimalPlaces = c.Byte(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.NumberFieldDefinitionID)
                .ForeignKey("dbo.FieldDefinitions", t => t.FieldDefinitionID)
                .Index(t => t.FieldDefinitionID);
            
            CreateTable(
                "dbo.Tbls",
                c => new
                    {
                        TblID = c.Int(nullable: false, identity: true),
                        PointsManagerID = c.Int(nullable: false),
                        DefaultRatingGroupAttributesID = c.Int(),
                        TblTabWord = c.String(maxLength: 50, unicode: false),
                        Name = c.String(),
                        TypeOfTblRow = c.String(maxLength: 50, unicode: false),
                        TblDimensionsID = c.Int(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                        AllowOverrideOfRatingGroupCharacterstics = c.Boolean(nullable: false),
                        AllowUsersToAddComments = c.Boolean(nullable: false),
                        LimitCommentsToUsersWhoCanMakeUserRatings = c.Boolean(nullable: false),
                        OneRatingPerRatingGroup = c.Boolean(nullable: false),
                        TblRowAdditionCriteria = c.String(),
                        SuppStylesHeader = c.String(),
                        SuppStylesMain = c.String(),
                        WidthStyleEntityCol = c.String(maxLength: 20),
                        WidthStyleNumCol = c.String(maxLength: 20),
                        FastTableSyncStatus = c.Byte(nullable: false),
                        NumTblRowsActive = c.Int(nullable: false),
                        NumTblRowsDeleted = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TblID)
                .ForeignKey("dbo.Users", t => t.Creator)
                .ForeignKey("dbo.TblDimensions", t => t.TblDimensionsID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .Index(t => t.PointsManagerID)
                .Index(t => t.TblDimensionsID)
                .Index(t => t.Creator);
            
            CreateTable(
                "dbo.ChangesGroup",
                c => new
                    {
                        ChangesGroupID = c.Int(nullable: false, identity: true),
                        PointsManagerID = c.Int(),
                        TblID = c.Int(),
                        Creator = c.Int(),
                        MakeChangeRatingID = c.Int(),
                        RewardRatingID = c.Int(),
                        StatusOfChanges = c.Byte(nullable: false),
                        ScheduleApprovalOrRejection = c.DateTime(),
                        ScheduleImplementation = c.DateTime(),
                    })
                .PrimaryKey(t => t.ChangesGroupID)
                .ForeignKey("dbo.Ratings", t => t.RewardRatingID)
                .ForeignKey("dbo.Users", t => t.Creator)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .ForeignKey("dbo.Tbls", t => t.TblID)
                .Index(t => t.PointsManagerID)
                .Index(t => t.TblID)
                .Index(t => t.Creator)
                .Index(t => t.RewardRatingID);
            
            CreateTable(
                "dbo.ChangesStatusOfObject",
                c => new
                    {
                        ChangesStatusOfObjectID = c.Int(nullable: false, identity: true),
                        ChangesGroupID = c.Int(nullable: false),
                        ObjectType = c.Byte(nullable: false),
                        AddObject = c.Boolean(nullable: false),
                        DeleteObject = c.Boolean(nullable: false),
                        ReplaceObject = c.Boolean(nullable: false),
                        ChangeName = c.Boolean(nullable: false),
                        ChangeOther = c.Boolean(nullable: false),
                        ChangeSetting1 = c.Boolean(nullable: false),
                        ChangeSetting2 = c.Boolean(nullable: false),
                        MayAffectRunningRating = c.Boolean(nullable: false),
                        NewName = c.String(maxLength: 50),
                        NewObject = c.Int(),
                        ExistingObject = c.Int(),
                        NewValueBoolean = c.Boolean(),
                        NewValueInteger = c.Int(),
                        NewValueDecimal = c.Decimal(precision: 18, scale: 4),
                        NewValueText = c.String(),
                        NewValueDateTime = c.DateTime(),
                        ChangeDescription = c.String(),
                    })
                .PrimaryKey(t => t.ChangesStatusOfObjectID)
                .ForeignKey("dbo.ChangesGroup", t => t.ChangesGroupID)
                .Index(t => t.ChangesGroupID);
            
            CreateTable(
                "dbo.PointsManagers",
                c => new
                    {
                        PointsManagerID = c.Int(nullable: false, identity: true),
                        DomainID = c.Int(nullable: false),
                        TrustTrackerUnitID = c.Int(),
                        CurrentPeriodDollarSubsidy = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndOfDollarSubsidyPeriod = c.DateTime(),
                        NextPeriodDollarSubsidy = c.Decimal(precision: 18, scale: 2),
                        NextPeriodLength = c.Int(),
                        NumPrizes = c.Short(nullable: false),
                        MinimumPayment = c.Decimal(precision: 18, scale: 2),
                        TotalUserPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CurrentUserPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CurrentUserPendingPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CurrentUserNotYetPendingPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CurrentPointsToCount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NumUsersMeetingUltimateStandard = c.Int(nullable: false),
                        NumUsersMeetingCurrentStandard = c.Int(nullable: false),
                        HighStakesProbability = c.Decimal(nullable: false, precision: 18, scale: 4),
                        HighStakesSecretMultiplier = c.Decimal(nullable: false, precision: 18, scale: 4),
                        HighStakesKnownMultiplier = c.Decimal(precision: 18, scale: 4),
                        HighStakesNoviceOn = c.Boolean(nullable: false),
                        HighStakesNoviceNumAutomatic = c.Int(nullable: false),
                        HighStakesNoviceNumOneThird = c.Int(nullable: false),
                        HighStakesNoviceNumOneTenth = c.Int(nullable: false),
                        DatabaseChangeSelectHighStakesNoviceNumPct = c.Decimal(nullable: false, precision: 18, scale: 4),
                        HighStakesNoviceNumActive = c.Int(nullable: false),
                        HighStakesNoviceTargetNum = c.Int(nullable: false),
                        DollarValuePerPoint = c.Decimal(nullable: false, precision: 18, scale: 8),
                        DiscountForGuarantees = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaximumTotalGuarantees = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaximumGuaranteePaymentPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalUnconditionalGuaranteesEarnedEver = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalConditionalGuaranteesEarnedEver = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalConditionalGuaranteesPending = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AllowApplicationsWhenNoConditionalGuaranteesAvailable = c.Boolean(nullable: false),
                        ConditionalGuaranteesAvailableForNewUsers = c.Boolean(nullable: false),
                        ConditionalGuaranteesAvailableForExistingUsers = c.Boolean(nullable: false),
                        ConditionalGuaranteeTimeBlockInHours = c.Int(nullable: false),
                        ConditionalGuaranteeApplicationsReceived = c.Int(nullable: false),
                        Name = c.String(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.PointsManagerID)
                .ForeignKey("dbo.TrustTrackerUnits", t => t.TrustTrackerUnitID)
                .ForeignKey("dbo.Domains", t => t.DomainID)
                .Index(t => t.DomainID)
                .Index(t => t.TrustTrackerUnitID);
            
            CreateTable(
                "dbo.AdministrationRightsGroups",
                c => new
                    {
                        AdministrationRightsGroupID = c.Int(nullable: false, identity: true),
                        PointsManagerID = c.Int(),
                        Name = c.String(nullable: false),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.AdministrationRightsGroupID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .Index(t => t.PointsManagerID);
            
            CreateTable(
                "dbo.AdministrationRights",
                c => new
                    {
                        AdministrationRightID = c.Int(nullable: false, identity: true),
                        AdministrationRightsGroupID = c.Int(nullable: false),
                        UserActionID = c.Int(),
                        AllowUserToMakeImmediateChanges = c.Boolean(nullable: false),
                        AllowUserToMakeProposals = c.Boolean(nullable: false),
                        AllowUserToSeekRewards = c.Boolean(nullable: false),
                        AllowUserNotToSeekRewards = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.AdministrationRightID)
                .ForeignKey("dbo.UserActions", t => t.UserActionID)
                .ForeignKey("dbo.AdministrationRightsGroups", t => t.AdministrationRightsGroupID)
                .Index(t => t.AdministrationRightsGroupID)
                .Index(t => t.UserActionID);
            
            CreateTable(
                "dbo.UserActions",
                c => new
                    {
                        UserActionID = c.Int(nullable: false, identity: true),
                        Text = c.String(unicode: false),
                        SuperUser = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserActionID);
            
            CreateTable(
                "dbo.ProposalEvaluationRatingSettings",
                c => new
                    {
                        ProposalEvaluationRatingSettingsID = c.Int(nullable: false, identity: true),
                        PointsManagerID = c.Int(),
                        UserActionID = c.Int(),
                        RatingGroupAttributesID = c.Int(nullable: false),
                        MinValueToApprove = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaxValueToReject = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TimeRequiredBeyondThreshold = c.Int(nullable: false),
                        MinProportionOfThisTime = c.Decimal(nullable: false, precision: 18, scale: 4),
                        HalfLifeForResolvingAtFinalValue = c.Int(nullable: false),
                        Name = c.String(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ProposalEvaluationRatingSettingsID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .ForeignKey("dbo.RatingGroupAttributes", t => t.RatingGroupAttributesID)
                .ForeignKey("dbo.UserActions", t => t.UserActionID)
                .Index(t => t.PointsManagerID)
                .Index(t => t.UserActionID)
                .Index(t => t.RatingGroupAttributesID);
            
            CreateTable(
                "dbo.RatingGroupAttributes",
                c => new
                    {
                        RatingGroupAttributesID = c.Int(nullable: false, identity: true),
                        RatingCharacteristicsID = c.Int(nullable: false),
                        RatingConditionID = c.Int(),
                        PointsManagerID = c.Int(),
                        ConstrainedSum = c.Decimal(precision: 18, scale: 4),
                        Name = c.String(),
                        TypeOfRatingGroup = c.Byte(),
                        Description = c.String(unicode: false),
                        RatingEndingTimeVaries = c.Boolean(nullable: false),
                        RatingsCanBeAutocalculated = c.Boolean(nullable: false),
                        LongTermPointsWeight = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MinimumDaysToTrackLongTerm = c.Int(nullable: false),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RatingGroupAttributesID)
                .ForeignKey("dbo.RatingCharacteristics", t => t.RatingCharacteristicsID)
                .ForeignKey("dbo.RatingConditions", t => t.RatingConditionID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .Index(t => t.RatingCharacteristicsID)
                .Index(t => t.RatingConditionID)
                .Index(t => t.PointsManagerID);
            
            CreateTable(
                "dbo.OverrideCharacteristics",
                c => new
                    {
                        OverrideCharacteristicsID = c.Int(nullable: false, identity: true),
                        RatingGroupAttributesID = c.Int(nullable: false),
                        TblRowID = c.Int(nullable: false),
                        TblColumnID = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.OverrideCharacteristicsID)
                .ForeignKey("dbo.TblColumns", t => t.TblColumnID)
                .ForeignKey("dbo.TblRows", t => t.TblRowID)
                .ForeignKey("dbo.RatingGroupAttributes", t => t.RatingGroupAttributesID)
                .Index(t => t.RatingGroupAttributesID)
                .Index(t => t.TblRowID)
                .Index(t => t.TblColumnID);
            
            CreateTable(
                "dbo.TblColumns",
                c => new
                    {
                        TblColumnID = c.Int(nullable: false, identity: true),
                        TblTabID = c.Int(nullable: false),
                        DefaultRatingGroupAttributesID = c.Int(nullable: false),
                        ConditionTblColumnID = c.Int(),
                        TrustTrackerUnitID = c.Int(),
                        ConditionGreaterThan = c.Decimal(precision: 18, scale: 4),
                        ConditionLessThan = c.Decimal(precision: 18, scale: 4),
                        CategoryNum = c.Int(nullable: false),
                        Abbreviation = c.String(maxLength: 20, fixedLength: true),
                        Name = c.String(maxLength: 50),
                        Explanation = c.String(),
                        WidthStyle = c.String(maxLength: 20),
                        NumNonNull = c.Int(nullable: false),
                        ProportionNonNull = c.Double(nullable: false),
                        UsingNonSparseColumn = c.Boolean(nullable: false),
                        ShouldUseNonSparseColumn = c.Boolean(nullable: false),
                        UseAsFilter = c.Boolean(nullable: false),
                        Sortable = c.Boolean(nullable: false),
                        DefaultSortOrderAscending = c.Boolean(nullable: false),
                        AutomaticallyCreateMissingRatings = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.TblColumnID)
                .ForeignKey("dbo.TrustTrackerUnits", t => t.TrustTrackerUnitID)
                .ForeignKey("dbo.TblColumns", t => t.ConditionTblColumnID)
                .ForeignKey("dbo.TblTabs", t => t.TblTabID, cascadeDelete: true)
                .ForeignKey("dbo.RatingGroupAttributes", t => t.DefaultRatingGroupAttributesID)
                .Index(t => t.TblTabID)
                .Index(t => t.DefaultRatingGroupAttributesID)
                .Index(t => t.ConditionTblColumnID)
                .Index(t => t.TrustTrackerUnitID);
            
            CreateTable(
                "dbo.RatingGroups",
                c => new
                    {
                        RatingGroupID = c.Int(nullable: false, identity: true),
                        RatingGroupAttributesID = c.Int(nullable: false),
                        TblRowID = c.Int(nullable: false),
                        TblColumnID = c.Int(nullable: false),
                        CurrentValueOfFirstRating = c.Decimal(precision: 18, scale: 4),
                        ValueRecentlyChanged = c.Boolean(nullable: false),
                        ResolutionTime = c.DateTime(),
                        TypeOfRatingGroup = c.Byte(nullable: false),
                        Status = c.Byte(nullable: false),
                        HighStakesKnown = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RatingGroupID)
                .ForeignKey("dbo.TblRows", t => t.TblRowID)
                .ForeignKey("dbo.TblColumns", t => t.TblColumnID)
                .ForeignKey("dbo.RatingGroupAttributes", t => t.RatingGroupAttributesID)
                .Index(t => t.RatingGroupAttributesID)
                .Index(t => t.TblRowID)
                .Index(t => t.TblColumnID);
            
            CreateTable(
                "dbo.RatingGroupPhaseStatus",
                c => new
                    {
                        RatingGroupPhaseStatusID = c.Int(nullable: false, identity: true),
                        RatingPhaseGroupID = c.Int(nullable: false),
                        RatingPhaseID = c.Int(nullable: false),
                        RatingGroupID = c.Int(nullable: false),
                        RoundNum = c.Int(nullable: false),
                        RoundNumThisPhase = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EarliestCompleteTime = c.DateTime(nullable: false),
                        ActualCompleteTime = c.DateTime(nullable: false),
                        ShortTermResolveTime = c.DateTime(nullable: false),
                        HighStakesSecret = c.Boolean(nullable: false),
                        HighStakesKnown = c.Boolean(nullable: false),
                        HighStakesReflected = c.Boolean(nullable: false),
                        HighStakesNoviceUser = c.Boolean(nullable: false),
                        HighStakesBecomeKnown = c.DateTime(),
                        HighStakesNoviceUserAfter = c.DateTime(),
                        DeletionTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.RatingGroupPhaseStatusID)
                .ForeignKey("dbo.RatingPhases", t => t.RatingPhaseID)
                .ForeignKey("dbo.RatingPhaseGroups", t => t.RatingPhaseGroupID)
                .ForeignKey("dbo.RatingGroups", t => t.RatingGroupID)
                .Index(t => t.RatingPhaseGroupID)
                .Index(t => t.RatingPhaseID)
                .Index(t => t.RatingGroupID);
            
            CreateTable(
                "dbo.RatingPhases",
                c => new
                    {
                        RatingPhaseID = c.Int(nullable: false, identity: true),
                        RatingPhaseGroupID = c.Int(nullable: false),
                        NumberInGroup = c.Int(nullable: false),
                        SubsidyLevel = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScoringRule = c.Short(nullable: false),
                        Timed = c.Boolean(nullable: false),
                        BaseTimingOnSpecificTime = c.Boolean(nullable: false),
                        EndTime = c.DateTime(),
                        RunTime = c.Int(),
                        HalfLifeForResolution = c.Int(nullable: false),
                        RepeatIndefinitely = c.Boolean(nullable: false),
                        RepeatNTimes = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RatingPhaseID)
                .ForeignKey("dbo.RatingPhaseGroups", t => t.RatingPhaseGroupID)
                .Index(t => t.RatingPhaseGroupID);
            
            CreateTable(
                "dbo.RatingPhaseGroups",
                c => new
                    {
                        RatingPhaseGroupID = c.Int(nullable: false, identity: true),
                        NumPhases = c.Int(nullable: false),
                        Name = c.String(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RatingPhaseGroupID)
                .ForeignKey("dbo.Users", t => t.Creator)
                .Index(t => t.Creator);
            
            CreateTable(
                "dbo.RatingCharacteristics",
                c => new
                    {
                        RatingCharacteristicsID = c.Int(nullable: false, identity: true),
                        RatingPhaseGroupID = c.Int(nullable: false),
                        SubsidyDensityRangeGroupID = c.Int(),
                        MinimumUserRating = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaximumUserRating = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DecimalPlaces = c.Byte(nullable: false),
                        Name = c.String(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RatingCharacteristicsID)
                .ForeignKey("dbo.Users", t => t.Creator)
                .ForeignKey("dbo.SubsidyDensityRangeGroups", t => t.SubsidyDensityRangeGroupID)
                .ForeignKey("dbo.RatingPhaseGroups", t => t.RatingPhaseGroupID)
                .Index(t => t.RatingPhaseGroupID)
                .Index(t => t.SubsidyDensityRangeGroupID)
                .Index(t => t.Creator);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        RatingID = c.Int(nullable: false, identity: true),
                        RatingGroupID = c.Int(nullable: false),
                        RatingCharacteristicsID = c.Int(nullable: false),
                        OwnedRatingGroupID = c.Int(),
                        TopmostRatingGroupID = c.Int(nullable: false),
                        MostRecentUserRatingID = c.Int(),
                        NumInGroup = c.Int(nullable: false),
                        TotalUserRatings = c.Int(nullable: false),
                        Name = c.String(),
                        Creator = c.Int(),
                        CurrentValue = c.Decimal(precision: 18, scale: 4),
                        LastTrustedValue = c.Decimal(precision: 18, scale: 4),
                        LastModifiedResolutionTimeOrCurrentValue = c.DateTime(nullable: false),
                        ReviewRecentUserRatingsAfter = c.DateTime(),
                    })
                .PrimaryKey(t => t.RatingID)
                .ForeignKey("dbo.UserRatings", t => t.MostRecentUserRatingID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.Creator)
                .ForeignKey("dbo.RatingCharacteristics", t => t.RatingCharacteristicsID)
                .ForeignKey("dbo.RatingGroups", t => t.RatingGroupID)
                .ForeignKey("dbo.RatingGroups", t => t.OwnedRatingGroupID)
                .ForeignKey("dbo.RatingGroups", t => t.TopmostRatingGroupID)
                .Index(t => t.RatingGroupID)
                .Index(t => t.RatingCharacteristicsID)
                .Index(t => t.OwnedRatingGroupID)
                .Index(t => t.TopmostRatingGroupID)
                .Index(t => t.MostRecentUserRatingID)
                .Index(t => t.Creator);
            
            CreateTable(
                "dbo.RatingConditions",
                c => new
                    {
                        RatingConditionID = c.Int(nullable: false, identity: true),
                        ConditionRatingID = c.Int(),
                        GreaterThan = c.Decimal(precision: 18, scale: 4),
                        LessThan = c.Decimal(precision: 18, scale: 4),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RatingConditionID)
                .ForeignKey("dbo.Ratings", t => t.ConditionRatingID)
                .Index(t => t.ConditionRatingID);
            
            CreateTable(
                "dbo.RatingPhaseStatus",
                c => new
                    {
                        RatingPhaseStatusID = c.Int(nullable: false, identity: true),
                        RatingGroupPhaseStatusID = c.Int(nullable: false),
                        RatingID = c.Int(nullable: false),
                        ShortTermResolutionValue = c.Decimal(precision: 18, scale: 4),
                        NumUserRatingsMadeDuringPhase = c.Int(nullable: false),
                        TriggerUserRatingsUpdate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RatingPhaseStatusID)
                .ForeignKey("dbo.Ratings", t => t.RatingID)
                .ForeignKey("dbo.RatingGroupPhaseStatus", t => t.RatingGroupPhaseStatusID)
                .Index(t => t.RatingGroupPhaseStatusID)
                .Index(t => t.RatingID);
            
            CreateTable(
                "dbo.UserRatings",
                c => new
                    {
                        UserRatingID = c.Int(nullable: false, identity: true),
                        UserRatingGroupID = c.Int(nullable: false),
                        RatingID = c.Int(nullable: false),
                        RatingPhaseStatusID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        TrustTrackerUnitID = c.Int(),
                        RewardPendingPointsTrackerID = c.Int(),
                        MostRecentUserRatingID = c.Int(),
                        PreviousRatingOrVirtualRating = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PreviousDisplayedRating = c.Decimal(precision: 18, scale: 4),
                        EnteredUserRating = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NewUserRating = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OriginalAdjustmentPct = c.Decimal(nullable: false, precision: 7, scale: 4),
                        OriginalTrustLevel = c.Decimal(nullable: false, precision: 7, scale: 4),
                        MaxGain = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaxLoss = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PotentialPointsShortTerm = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PotentialPointsLongTerm = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PotentialPointsLongTermUnweighted = c.Decimal(nullable: false, precision: 18, scale: 4),
                        LongTermPointsWeight = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PointsPumpingProportion = c.Decimal(precision: 18, scale: 4),
                        PastPointsPumpingProportion = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PercentPreviousRatings = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsTrusted = c.Boolean(nullable: false),
                        MadeDirectly = c.Boolean(nullable: false),
                        LongTermResolutionReflected = c.Boolean(nullable: false),
                        ShortTermResolutionReflected = c.Boolean(nullable: false),
                        PointsHaveBecomePending = c.Boolean(nullable: false),
                        ForceRecalculate = c.Boolean(nullable: false),
                        HighStakesPreviouslySecret = c.Boolean(nullable: false),
                        HighStakesKnown = c.Boolean(nullable: false),
                        PreviouslyRated = c.Boolean(nullable: false),
                        SubsequentlyRated = c.Boolean(nullable: false),
                        IsMostRecent10Pct = c.Boolean(nullable: false),
                        IsMostRecent30Pct = c.Boolean(nullable: false),
                        IsMostRecent70Pct = c.Boolean(nullable: false),
                        IsMostRecent90Pct = c.Boolean(nullable: false),
                        IsUsersFirstWeek = c.Boolean(nullable: false),
                        LogarithmicBase = c.Decimal(precision: 18, scale: 4),
                        HighStakesMultiplierOverride = c.Decimal(precision: 18, scale: 4),
                        WhenPointsBecomePending = c.DateTime(),
                        LastModifiedTime = c.DateTime(nullable: false),
                        VolatilityTrackingNextTimeFrameToRemove = c.Byte(nullable: false),
                        LastWeekDistanceFromStart = c.Decimal(nullable: false, precision: 18, scale: 4),
                        LastWeekPushback = c.Decimal(nullable: false, precision: 18, scale: 4),
                        LastYearPushback = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UserRatingNumberForUser = c.Int(nullable: false),
                        NextRecencyUpdateAtUserRatingNum = c.Int(),
                    })
                .PrimaryKey(t => t.UserRatingID)
                .ForeignKey("dbo.TrustTrackerUnits", t => t.TrustTrackerUnitID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .ForeignKey("dbo.RewardPendingPointsTrackers", t => t.RewardPendingPointsTrackerID)
                .ForeignKey("dbo.UserRatingGroups", t => t.UserRatingGroupID)
                .ForeignKey("dbo.UserRatings", t => t.MostRecentUserRatingID)
                .ForeignKey("dbo.RatingPhaseStatus", t => t.RatingPhaseStatusID)
                .ForeignKey("dbo.Ratings", t => t.RatingID)
                .Index(t => t.UserRatingGroupID)
                .Index(t => t.RatingID)
                .Index(t => t.RatingPhaseStatusID)
                .Index(t => t.UserID)
                .Index(t => t.TrustTrackerUnitID)
                .Index(t => t.RewardPendingPointsTrackerID)
                .Index(t => t.MostRecentUserRatingID);
            
            CreateTable(
                "dbo.RewardPendingPointsTrackers",
                c => new
                    {
                        RewardPendingPointsTrackerID = c.Int(nullable: false, identity: true),
                        PendingRating = c.Decimal(precision: 18, scale: 4),
                        TimeOfPendingRating = c.DateTime(),
                        RewardTblRowID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RewardPendingPointsTrackerID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .ForeignKey("dbo.TblRows", t => t.RewardTblRowID)
                .Index(t => t.RewardTblRowID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.TblRows",
                c => new
                    {
                        TblRowID = c.Int(nullable: false, identity: true),
                        TblID = c.Int(nullable: false),
                        TblRowFieldDisplayID = c.Int(nullable: false),
                        Name = c.String(),
                        Status = c.Byte(nullable: false),
                        StatusRecentlyChanged = c.Boolean(nullable: false),
                        CountHighStakesNow = c.Int(nullable: false),
                        CountNonnullEntries = c.Int(nullable: false),
                        CountUserPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ElevateOnMostNeedsRating = c.Boolean(nullable: false),
                        InitialFieldsDisplaySet = c.Boolean(nullable: false),
                        FastAccessInitialCopy = c.Boolean(nullable: false),
                        FastAccessDeleteThenRecopy = c.Boolean(nullable: false),
                        FastAccessUpdateFields = c.Boolean(nullable: false),
                        FastAccessUpdateRatings = c.Boolean(nullable: false),
                        FastAccessUpdateSpecified = c.Boolean(nullable: false),
                        FastAccessUpdated = c.Binary(),
                    })
                .PrimaryKey(t => t.TblRowID)
                .ForeignKey("dbo.TblRowFieldDisplays", t => t.TblRowFieldDisplayID)
                .ForeignKey("dbo.Tbls", t => t.TblID)
                .Index(t => t.TblID)
                .Index(t => t.TblRowFieldDisplayID);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentsID = c.Int(nullable: false, identity: true),
                        TblRowID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        CommentTitle = c.String(nullable: false, unicode: false),
                        CommentText = c.String(nullable: false, unicode: false),
                        DateTime = c.DateTime(nullable: false),
                        LastDeletedDate = c.DateTime(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.CommentsID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .ForeignKey("dbo.TblRows", t => t.TblRowID)
                .Index(t => t.TblRowID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 50),
                        SuperUser = c.Boolean(nullable: false),
                        TrustPointsRatioTotals = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.PointsAdjustments",
                c => new
                    {
                        PointsAdjustmentID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        PointsManagerID = c.Int(nullable: false),
                        Reason = c.Int(nullable: false),
                        TotalAdjustment = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CurrentAdjustment = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CashValue = c.Decimal(precision: 18, scale: 2),
                        WhenMade = c.DateTime(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.PointsAdjustmentID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .Index(t => t.UserID)
                .Index(t => t.PointsManagerID);
            
            CreateTable(
                "dbo.PointsTotals",
                c => new
                    {
                        PointsTotalID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        PointsManagerID = c.Int(nullable: false),
                        CurrentPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PotentialMaxLossOnNotYetPending = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PendingPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NotYetPendingPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TrustPoints = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TrustPointsRatio = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NumPendingOrFinalizedRatings = c.Int(nullable: false),
                        PointsPerRating = c.Decimal(nullable: false, precision: 18, scale: 4),
                        FirstUserRating = c.DateTime(),
                        LastCheckIn = c.DateTime(),
                        CurrentCheckInPeriodStart = c.DateTime(),
                        TotalTimeThisCheckInPeriod = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalTimeThisRewardPeriod = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalTimeEver = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PointsPerHour = c.Decimal(precision: 18, scale: 4),
                        ProjectedPointsPerHour = c.Decimal(precision: 18, scale: 4),
                        GuaranteedPaymentsEarnedThisRewardPeriod = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PendingConditionalGuaranteeApplication = c.String(maxLength: 50),
                        PendingConditionalGuaranteePayment = c.Decimal(precision: 18, scale: 2),
                        PendingConditionalGuaranteeTotalHoursAtStart = c.Decimal(precision: 18, scale: 4),
                        PendingConditionalGuaranteeTotalHoursNeeded = c.Decimal(precision: 18, scale: 4),
                        PendingConditionalGuaranteePaymentAlreadyMade = c.Decimal(precision: 18, scale: 2),
                        RequestConditionalGuaranteeWhenAvailableTimeRequestMade = c.DateTime(),
                        TotalPointsOrPendingPointsLongTermUnweighted = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PointsPerRatingLongTerm = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PointsPumpingProportionAvg_Numer = c.Single(nullable: false),
                        PointsPumpingProportionAvg_Denom = c.Single(nullable: false),
                        PointsPumpingProportionAvg = c.Single(nullable: false),
                        NumUserRatings = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PointsTotalID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .Index(t => t.UserID)
                .Index(t => t.PointsManagerID);
            
            CreateTable(
                "dbo.RatingGroupResolutions",
                c => new
                    {
                        RatingGroupResolutionID = c.Int(nullable: false, identity: true),
                        RatingGroupID = c.Int(nullable: false),
                        CancelPreviousResolutions = c.Boolean(nullable: false),
                        ResolveByUnwinding = c.Boolean(nullable: false),
                        EffectiveTime = c.DateTime(nullable: false),
                        ExecutionTime = c.DateTime(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RatingGroupResolutionID)
                .ForeignKey("dbo.Users", t => t.Creator)
                .ForeignKey("dbo.RatingGroups", t => t.RatingGroupID)
                .Index(t => t.RatingGroupID)
                .Index(t => t.Creator);
            
            CreateTable(
                "dbo.RatingPlans",
                c => new
                    {
                        RatingPlansID = c.Int(nullable: false, identity: true),
                        RatingGroupAttributesID = c.Int(nullable: false),
                        NumInGroup = c.Int(nullable: false),
                        OwnedRatingGroupAttributesID = c.Int(),
                        DefaultUserRating = c.Decimal(precision: 18, scale: 4),
                        Name = c.String(),
                        Description = c.String(unicode: false),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RatingPlansID)
                .ForeignKey("dbo.Users", t => t.Creator)
                .ForeignKey("dbo.RatingGroupAttributes", t => t.RatingGroupAttributesID)
                .ForeignKey("dbo.RatingGroupAttributes", t => t.OwnedRatingGroupAttributesID)
                .Index(t => t.RatingGroupAttributesID)
                .Index(t => t.OwnedRatingGroupAttributesID)
                .Index(t => t.Creator);
            
            CreateTable(
                "dbo.SubsidyDensityRangeGroups",
                c => new
                    {
                        SubsidyDensityRangeGroupID = c.Int(nullable: false, identity: true),
                        UseLogarithmBase = c.Decimal(precision: 18, scale: 4),
                        CumDensityTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Name = c.String(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.SubsidyDensityRangeGroupID)
                .ForeignKey("dbo.Users", t => t.Creator)
                .Index(t => t.Creator);
            
            CreateTable(
                "dbo.SubsidyDensityRanges",
                c => new
                    {
                        SubsidyDensityRangeID = c.Int(nullable: false, identity: true),
                        SubsidyDensityRangeGroupID = c.Int(nullable: false),
                        RangeBottom = c.Decimal(nullable: false, precision: 18, scale: 4),
                        RangeTop = c.Decimal(nullable: false, precision: 18, scale: 4),
                        LiquidityFactor = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CumDensityBottom = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CumDensityTop = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.SubsidyDensityRangeID)
                .ForeignKey("dbo.SubsidyDensityRangeGroups", t => t.SubsidyDensityRangeGroupID)
                .Index(t => t.SubsidyDensityRangeGroupID);
            
            CreateTable(
                "dbo.TrustTrackerForChoiceInGroups",
                c => new
                    {
                        TrustTrackerForChoiceInGroupID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        ChoiceInGroupID = c.Int(nullable: false),
                        TblID = c.Int(nullable: false),
                        SumAdjustmentPctTimesRatingMagnitude = c.Single(nullable: false),
                        SumRatingMagnitudes = c.Single(nullable: false),
                        TrustLevelForChoice = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.TrustTrackerForChoiceInGroupID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .ForeignKey("dbo.Tbls", t => t.TblID)
                .ForeignKey("dbo.ChoiceInGroups", t => t.ChoiceInGroupID)
                .Index(t => t.UserID)
                .Index(t => t.ChoiceInGroupID)
                .Index(t => t.TblID);
            
            CreateTable(
                "dbo.TrustTrackerForChoiceInGroupsUserRatingLinks",
                c => new
                    {
                        TrustTrackerForChoiceInGroupsUserRatingLinkID = c.Int(nullable: false, identity: true),
                        UserRatingID = c.Int(nullable: false),
                        TrustTrackerForChoiceInGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrustTrackerForChoiceInGroupsUserRatingLinkID)
                .ForeignKey("dbo.TrustTrackerForChoiceInGroups", t => t.TrustTrackerForChoiceInGroupID)
                .ForeignKey("dbo.UserRatings", t => t.UserRatingID)
                .Index(t => t.UserRatingID)
                .Index(t => t.TrustTrackerForChoiceInGroupID);
            
            CreateTable(
                "dbo.TrustTrackers",
                c => new
                    {
                        TrustTrackerID = c.Int(nullable: false, identity: true),
                        TrustTrackerUnitID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        OverallTrustLevel = c.Double(nullable: false),
                        OverallTrustLevelAtLastReview = c.Double(nullable: false),
                        DeltaOverallTrustLevel = c.Double(nullable: false),
                        SkepticalTrustLevel = c.Double(nullable: false),
                        SumUserInteractionWeights = c.Double(nullable: false),
                        EgalitarianTrustLevel = c.Double(nullable: false),
                        Egalitarian_Num = c.Double(nullable: false),
                        Egalitarian_Denom = c.Double(nullable: false),
                        EgalitarianTrustLevelOverride = c.Double(),
                        MustUpdateUserInteractionEgalitarianTrustLevel = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TrustTrackerID)
                .ForeignKey("dbo.TrustTrackerUnits", t => t.TrustTrackerUnitID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.TrustTrackerUnitID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.TrustTrackerStats",
                c => new
                    {
                        TrustTrackerStatID = c.Int(nullable: false, identity: true),
                        TrustTrackerID = c.Int(nullable: false),
                        StatNum = c.Short(nullable: false),
                        TrustValue = c.Double(nullable: false),
                        Trust_Numer = c.Double(nullable: false),
                        Trust_Denom = c.Double(nullable: false),
                        SumUserInteractionStatWeights = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TrustTrackerStatID)
                .ForeignKey("dbo.TrustTrackers", t => t.TrustTrackerID)
                .Index(t => t.TrustTrackerID);
            
            CreateTable(
                "dbo.UserInteractionStats",
                c => new
                    {
                        UserInteractionStatID = c.Int(nullable: false, identity: true),
                        UserInteractionID = c.Int(nullable: false),
                        TrustTrackerStatID = c.Int(nullable: false),
                        StatNum = c.Short(nullable: false),
                        SumAdjustPctTimesWeight = c.Double(nullable: false),
                        SumWeights = c.Double(nullable: false),
                        AvgAdjustmentPctWeighted = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.UserInteractionStatID)
                .ForeignKey("dbo.UserInteractions", t => t.UserInteractionID, cascadeDelete: true)
                .ForeignKey("dbo.TrustTrackerStats", t => t.TrustTrackerStatID)
                .Index(t => t.UserInteractionID)
                .Index(t => t.TrustTrackerStatID);
            
            CreateTable(
                "dbo.UserInteractions",
                c => new
                    {
                        UserInteractionID = c.Int(nullable: false, identity: true),
                        TrustTrackerUnitID = c.Int(nullable: false),
                        OrigRatingUserID = c.Int(nullable: false),
                        LatestRatingUserID = c.Int(nullable: false),
                        NumTransactions = c.Int(nullable: false),
                        LatestUserEgalitarianTrust = c.Double(nullable: false),
                        WeightInCalculatingTrustTotal = c.Double(nullable: false),
                        LatestUserEgalitarianTrustAtLastWeightUpdate = c.Double(),
                    })
                .PrimaryKey(t => t.UserInteractionID)
                .ForeignKey("dbo.TrustTrackerUnits", t => t.TrustTrackerUnitID)
                .ForeignKey("dbo.Users", t => t.OrigRatingUserID)
                .ForeignKey("dbo.Users", t => t.LatestRatingUserID)
                .Index(t => t.TrustTrackerUnitID)
                .Index(t => t.OrigRatingUserID)
                .Index(t => t.LatestRatingUserID);
            
            CreateTable(
                "dbo.TrustTrackerUnits",
                c => new
                    {
                        TrustTrackerUnitID = c.Int(nullable: false, identity: true),
                        SkepticalTrustThreshhold = c.Short(nullable: false),
                        LastSkepticalTrustThreshhold = c.Short(nullable: false),
                        MinUpdateIntervalSeconds = c.Int(nullable: false),
                        MaxUpdateIntervalSeconds = c.Int(nullable: false),
                        ExtendIntervalWhenChangeIsLessThanThis = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExtendIntervalMultiplier = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.TrustTrackerUnitID);
            
            CreateTable(
                "dbo.UserCheckIns",
                c => new
                    {
                        UserCheckInID = c.Int(nullable: false, identity: true),
                        CheckInTime = c.DateTime(nullable: false),
                        UserID = c.Int(),
                    })
                .PrimaryKey(t => t.UserCheckInID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.UserInfo",
                c => new
                    {
                        UserInfoID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        FirstName = c.String(maxLength: 50, unicode: false),
                        LastName = c.String(maxLength: 50, unicode: false),
                        Email = c.String(maxLength: 250, unicode: false),
                        Address1 = c.String(maxLength: 200, unicode: false),
                        Address2 = c.String(maxLength: 200, unicode: false),
                        WorkPhone = c.String(maxLength: 50, unicode: false),
                        HomePhone = c.String(maxLength: 50, unicode: false),
                        MobilePhone = c.String(maxLength: 50, unicode: false),
                        Password = c.String(maxLength: 50, unicode: false),
                        ZipCode = c.String(maxLength: 50, unicode: false),
                        City = c.String(maxLength: 50, unicode: false),
                        State = c.String(maxLength: 50, unicode: false),
                        Country = c.String(maxLength: 50, unicode: false),
                        IsVerified = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserInfoID)
                .ForeignKey("dbo.Users", t => t.UserInfoID)
                .Index(t => t.UserInfoID);
            
            CreateTable(
                "dbo.UserRatingsToAdd",
                c => new
                    {
                        UserRatingsToAddID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        TopRatingGroupID = c.Int(nullable: false),
                        UserRatingHierarchy = c.Binary(),
                    })
                .PrimaryKey(t => t.UserRatingsToAddID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .ForeignKey("dbo.RatingGroups", t => t.TopRatingGroupID)
                .Index(t => t.UserID)
                .Index(t => t.TopRatingGroupID);
            
            CreateTable(
                "dbo.UsersAdministrationRightsGroups",
                c => new
                    {
                        UsersAdministrationRightsGroupID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(),
                        PointsManagerID = c.Int(nullable: false),
                        AdministrationRightsGroupID = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.UsersAdministrationRightsGroupID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .ForeignKey("dbo.AdministrationRightsGroups", t => t.AdministrationRightsGroupID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .Index(t => t.UserID)
                .Index(t => t.PointsManagerID)
                .Index(t => t.AdministrationRightsGroupID);
            
            CreateTable(
                "dbo.UsersRights",
                c => new
                    {
                        UsersRightsID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(),
                        PointsManagerID = c.Int(),
                        MayView = c.Boolean(nullable: false),
                        MayPredict = c.Boolean(nullable: false),
                        MayAddTbls = c.Boolean(nullable: false),
                        MayResolveRatings = c.Boolean(nullable: false),
                        MayChangeTblRows = c.Boolean(nullable: false),
                        MayChangeChoiceGroups = c.Boolean(nullable: false),
                        MayChangeCharacteristics = c.Boolean(nullable: false),
                        MayChangeCategories = c.Boolean(nullable: false),
                        MayChangeUsersRights = c.Boolean(nullable: false),
                        MayAdjustPoints = c.Boolean(nullable: false),
                        MayChangeProposalSettings = c.Boolean(nullable: false),
                        Name = c.String(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.UsersRightsID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.UserID)
                .Index(t => t.PointsManagerID);
            
            CreateTable(
                "dbo.SearchWordTblRowNames",
                c => new
                    {
                        SearchWordTblRowNameID = c.Int(nullable: false, identity: true),
                        TblRowID = c.Int(nullable: false),
                        SearchWordID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SearchWordTblRowNameID)
                .ForeignKey("dbo.SearchWords", t => t.SearchWordID)
                .ForeignKey("dbo.TblRows", t => t.TblRowID)
                .Index(t => t.TblRowID)
                .Index(t => t.SearchWordID);
            
            CreateTable(
                "dbo.SearchWords",
                c => new
                    {
                        SearchWordID = c.Int(nullable: false, identity: true),
                        TheWord = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.SearchWordID);
            
            CreateTable(
                "dbo.SearchWordChoices",
                c => new
                    {
                        SearchWordChoiceID = c.Int(nullable: false, identity: true),
                        ChoiceInGroupID = c.Int(nullable: false),
                        SearchWordID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SearchWordChoiceID)
                .ForeignKey("dbo.SearchWords", t => t.SearchWordID)
                .ForeignKey("dbo.ChoiceInGroups", t => t.ChoiceInGroupID)
                .Index(t => t.ChoiceInGroupID)
                .Index(t => t.SearchWordID);
            
            CreateTable(
                "dbo.SearchWordHierarchyItems",
                c => new
                    {
                        SearchWordHierarchyItemID = c.Int(nullable: false, identity: true),
                        HierarchyItemID = c.Int(nullable: false),
                        SearchWordID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SearchWordHierarchyItemID)
                .ForeignKey("dbo.HierarchyItems", t => t.HierarchyItemID)
                .ForeignKey("dbo.SearchWords", t => t.SearchWordID)
                .Index(t => t.HierarchyItemID)
                .Index(t => t.SearchWordID);
            
            CreateTable(
                "dbo.HierarchyItems",
                c => new
                    {
                        HierarchyItemID = c.Int(nullable: false, identity: true),
                        HigherHierarchyItemID = c.Int(),
                        HigherHierarchyItemForRoutingID = c.Int(),
                        TblID = c.Int(),
                        HierarchyItemName = c.String(),
                        FullHierarchyWithHtml = c.String(),
                        FullHierarchyNoHtml = c.String(),
                        RouteToHere = c.String(),
                        IncludeInMenu = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.HierarchyItemID)
                .ForeignKey("dbo.HierarchyItems", t => t.HigherHierarchyItemForRoutingID)
                .ForeignKey("dbo.HierarchyItems", t => t.HigherHierarchyItemID)
                .ForeignKey("dbo.Tbls", t => t.TblID)
                .Index(t => t.HigherHierarchyItemID)
                .Index(t => t.HigherHierarchyItemForRoutingID)
                .Index(t => t.TblID);
            
            CreateTable(
                "dbo.SearchWordTextFields",
                c => new
                    {
                        SearchWordTextFieldID = c.Int(nullable: false, identity: true),
                        TextFieldID = c.Int(nullable: false),
                        SearchWordID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SearchWordTextFieldID)
                .ForeignKey("dbo.TextFields", t => t.TextFieldID)
                .ForeignKey("dbo.SearchWords", t => t.SearchWordID)
                .Index(t => t.TextFieldID)
                .Index(t => t.SearchWordID);
            
            CreateTable(
                "dbo.TextFields",
                c => new
                    {
                        TextFieldID = c.Int(nullable: false, identity: true),
                        FieldID = c.Int(nullable: false),
                        Text = c.String(),
                        Link = c.String(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.TextFieldID)
                .ForeignKey("dbo.Fields", t => t.FieldID)
                .Index(t => t.FieldID);
            
            CreateTable(
                "dbo.TblRowFieldDisplays",
                c => new
                    {
                        TblRowFieldDisplayID = c.Int(nullable: false, identity: true),
                        Row = c.String(),
                        PopUp = c.String(),
                        TblRowPage = c.String(),
                        ResetNeeded = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TblRowFieldDisplayID);
            
            CreateTable(
                "dbo.TblRowStatusRecord",
                c => new
                    {
                        RecordId = c.Int(nullable: false, identity: true),
                        TblRowId = c.Int(nullable: false),
                        TimeChanged = c.DateTime(nullable: false),
                        Adding = c.Boolean(nullable: false),
                        Deleting = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RecordId)
                .ForeignKey("dbo.TblRows", t => t.TblRowId)
                .Index(t => t.TblRowId);
            
            CreateTable(
                "dbo.VolatilityTblRowTrackers",
                c => new
                    {
                        VolatilityTblRowTrackerID = c.Int(nullable: false, identity: true),
                        TblRowID = c.Int(nullable: false),
                        DurationType = c.Byte(nullable: false),
                        TotalMovement = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DistanceFromStart = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Pushback = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PushbackProportion = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.VolatilityTblRowTrackerID)
                .ForeignKey("dbo.TblRows", t => t.TblRowID)
                .Index(t => t.TblRowID);
            
            CreateTable(
                "dbo.VolatilityTrackers",
                c => new
                    {
                        VolatilityTrackerID = c.Int(nullable: false, identity: true),
                        RatingGroupID = c.Int(nullable: false),
                        VolatilityTblRowTrackerID = c.Int(nullable: false),
                        DurationType = c.Byte(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        TotalMovement = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DistanceFromStart = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Pushback = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PushbackProportion = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.VolatilityTrackerID)
                .ForeignKey("dbo.VolatilityTblRowTrackers", t => t.VolatilityTblRowTrackerID)
                .ForeignKey("dbo.RatingGroups", t => t.RatingGroupID)
                .Index(t => t.RatingGroupID)
                .Index(t => t.VolatilityTblRowTrackerID);
            
            CreateTable(
                "dbo.UserRatingGroups",
                c => new
                    {
                        UserRatingGroupID = c.Int(nullable: false, identity: true),
                        RatingGroupID = c.Int(nullable: false),
                        RatingGroupPhaseStatusID = c.Int(nullable: false),
                        WhenMade = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserRatingGroupID)
                .ForeignKey("dbo.RatingGroupPhaseStatus", t => t.RatingGroupPhaseStatusID)
                .ForeignKey("dbo.RatingGroups", t => t.RatingGroupID)
                .Index(t => t.RatingGroupID)
                .Index(t => t.RatingGroupPhaseStatusID);
            
            CreateTable(
                "dbo.SubsidyAdjustments",
                c => new
                    {
                        SubsidyAdjustmentID = c.Int(nullable: false, identity: true),
                        RatingGroupPhaseStatusID = c.Int(nullable: false),
                        SubsidyAdjustmentFactor = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EffectiveTime = c.DateTime(nullable: false),
                        EndingTime = c.DateTime(),
                        EndingTimeHalfLife = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.SubsidyAdjustmentID)
                .ForeignKey("dbo.RatingGroupPhaseStatus", t => t.RatingGroupPhaseStatusID)
                .Index(t => t.RatingGroupPhaseStatusID);
            
            CreateTable(
                "dbo.RatingGroupStatusRecords",
                c => new
                    {
                        RatingGroupStatusRecordID = c.Int(nullable: false, identity: true),
                        RatingGroupID = c.Int(nullable: false),
                        OldValueOfFirstRating = c.Decimal(precision: 18, scale: 4, storeType: "numeric"),
                        NewValueTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RatingGroupStatusRecordID)
                .ForeignKey("dbo.RatingGroups", t => t.RatingGroupID)
                .Index(t => t.RatingGroupID);
            
            CreateTable(
                "dbo.TblColumnFormatting",
                c => new
                    {
                        TblColumnFormattingID = c.Int(nullable: false, identity: true),
                        TblColumnID = c.Int(nullable: false),
                        Prefix = c.String(maxLength: 10),
                        Suffix = c.String(maxLength: 10),
                        OmitLeadingZero = c.Boolean(nullable: false),
                        ExtraDecimalPlaceAbove = c.Decimal(precision: 18, scale: 4),
                        ExtraDecimalPlace2Above = c.Decimal(precision: 18, scale: 4),
                        ExtraDecimalPlace3Above = c.Decimal(precision: 18, scale: 4),
                        SuppStylesHeader = c.String(),
                        SuppStylesMain = c.String(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.TblColumnFormattingID)
                .ForeignKey("dbo.TblColumns", t => t.TblColumnID)
                .Index(t => t.TblColumnID);
            
            CreateTable(
                "dbo.TblTabs",
                c => new
                    {
                        TblTabID = c.Int(nullable: false, identity: true),
                        TblID = c.Int(nullable: false),
                        NumInTbl = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        DefaultSortTblColumnID = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.TblTabID)
                .ForeignKey("dbo.Tbls", t => t.TblID, cascadeDelete: true)
                .Index(t => t.TblID);
            
            CreateTable(
                "dbo.RewardRatingSettings",
                c => new
                    {
                        RewardRatingSettingsID = c.Int(nullable: false, identity: true),
                        PointsManagerID = c.Int(),
                        UserActionID = c.Int(),
                        RatingGroupAttributesID = c.Int(nullable: false),
                        ProbOfRewardEvaluation = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Multiplier = c.Decimal(precision: 18, scale: 4),
                        Name = c.String(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RewardRatingSettingsID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .ForeignKey("dbo.UserActions", t => t.UserActionID)
                .ForeignKey("dbo.RatingGroupAttributes", t => t.RatingGroupAttributesID)
                .Index(t => t.PointsManagerID)
                .Index(t => t.UserActionID)
                .Index(t => t.RatingGroupAttributesID);
            
            CreateTable(
                "dbo.Domains",
                c => new
                    {
                        DomainID = c.Int(nullable: false, identity: true),
                        ActiveRatingWebsite = c.Boolean(nullable: false),
                        Name = c.String(nullable: false),
                        TblDimensionsID = c.Int(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.DomainID)
                .ForeignKey("dbo.TblDimensions", t => t.TblDimensionsID)
                .Index(t => t.TblDimensionsID);
            
            CreateTable(
                "dbo.InsertableContents",
                c => new
                    {
                        InsertableContentID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, unicode: false),
                        DomainID = c.Int(),
                        PointsManagerID = c.Int(),
                        TblID = c.Int(),
                        Content = c.String(unicode: false),
                        IsTextOnly = c.Boolean(nullable: false),
                        Overridable = c.Boolean(nullable: false),
                        Location = c.Short(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.InsertableContentID)
                .ForeignKey("dbo.Domains", t => t.DomainID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .ForeignKey("dbo.Tbls", t => t.TblID)
                .Index(t => t.DomainID)
                .Index(t => t.PointsManagerID)
                .Index(t => t.TblID);
            
            CreateTable(
                "dbo.TblDimensions",
                c => new
                    {
                        TblDimensionsID = c.Int(nullable: false, identity: true),
                        MaxWidthOfImageInRowHeaderCell = c.Int(nullable: false),
                        MaxHeightOfImageInRowHeaderCell = c.Int(nullable: false),
                        MaxWidthOfImageInTblRowPopUpWindow = c.Int(nullable: false),
                        MaxHeightOfImageInTblRowPopUpWindow = c.Int(nullable: false),
                        WidthOfTblRowPopUpWindow = c.Int(nullable: false),
                        Creator = c.Int(),
                        Status = c.Byte(),
                    })
                .PrimaryKey(t => t.TblDimensionsID);
            
            CreateTable(
                "dbo.ProposalSettings",
                c => new
                    {
                        ProposalSettingsID = c.Int(nullable: false, identity: true),
                        PointsManagerID = c.Int(),
                        TblID = c.Int(),
                        UsersMayProposeAddingTbls = c.Boolean(nullable: false),
                        UsersMayProposeResolvingRatings = c.Boolean(nullable: false),
                        UsersMayProposeChangingTblRows = c.Boolean(nullable: false),
                        UsersMayProposeChangingChoiceGroups = c.Boolean(nullable: false),
                        UsersMayProposeChangingCharacteristics = c.Boolean(nullable: false),
                        UsersMayProposeChangingColumns = c.Boolean(nullable: false),
                        UsersMayProposeChangingUsersRights = c.Boolean(nullable: false),
                        UsersMayProposeAdjustingPoints = c.Boolean(nullable: false),
                        UsersMayProposeChangingProposalSettings = c.Boolean(nullable: false),
                        MinValueToApprove = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaxValueToReject = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MinTimePastThreshold = c.Int(nullable: false),
                        MinProportionOfThisTime = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MinAdditionalTimeForRewardRating = c.Int(nullable: false),
                        HalfLifeForRewardRating = c.Int(nullable: false),
                        MaxBonusForProposal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaxPenaltyForRejection = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SubsidyForApprovalRating = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SubsidyForRewardRating = c.Decimal(nullable: false, precision: 18, scale: 4),
                        HalfLifeForResolvingAtFinalValue = c.Int(nullable: false),
                        RequiredPointsToMakeProposal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Name = c.String(),
                        Creator = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ProposalSettingsID)
                .ForeignKey("dbo.PointsManagers", t => t.PointsManagerID)
                .ForeignKey("dbo.Tbls", t => t.TblID)
                .Index(t => t.PointsManagerID)
                .Index(t => t.TblID);
            
            CreateTable(
                "dbo.TextFieldDefinitions",
                c => new
                    {
                        TextFieldDefinitionID = c.Int(nullable: false, identity: true),
                        FieldDefinitionID = c.Int(nullable: false),
                        IncludeText = c.Boolean(nullable: false),
                        IncludeLink = c.Boolean(nullable: false),
                        Searchable = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.TextFieldDefinitionID)
                .ForeignKey("dbo.FieldDefinitions", t => t.FieldDefinitionID)
                .Index(t => t.FieldDefinitionID);
            
            CreateTable(
                "dbo.DateTimeFields",
                c => new
                    {
                        DateTimeFieldID = c.Int(nullable: false, identity: true),
                        FieldID = c.Int(nullable: false),
                        DateTime = c.DateTime(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.DateTimeFieldID)
                .ForeignKey("dbo.Fields", t => t.FieldID)
                .Index(t => t.FieldID);
            
            CreateTable(
                "dbo.NumberFields",
                c => new
                    {
                        NumberFieldID = c.Int(nullable: false, identity: true),
                        FieldID = c.Int(nullable: false),
                        Number = c.Decimal(precision: 18, scale: 4),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.NumberFieldID)
                .ForeignKey("dbo.Fields", t => t.FieldID)
                .Index(t => t.FieldID);
            
            CreateTable(
                "dbo.DatabaseStatus",
                c => new
                    {
                        DatabaseStatusID = c.Int(nullable: false, identity: true),
                        PreventChanges = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DatabaseStatusID);
            
            CreateTable(
                "dbo.InvitedUser",
                c => new
                    {
                        ActivationNumber = c.Int(nullable: false, identity: true),
                        EmailId = c.String(maxLength: 50),
                        MayView = c.Boolean(nullable: false),
                        MayPredict = c.Boolean(nullable: false),
                        MayAddTbls = c.Boolean(nullable: false),
                        MayResolveRatings = c.Boolean(nullable: false),
                        MayChangeTblRows = c.Boolean(nullable: false),
                        MayChangeChoiceGroups = c.Boolean(nullable: false),
                        MayChangeCharacteristics = c.Boolean(nullable: false),
                        MayChangeColumns = c.Boolean(nullable: false),
                        MayChangeUsersRights = c.Boolean(nullable: false),
                        MayAdjustPoints = c.Boolean(nullable: false),
                        MayChangeProposalSettings = c.Boolean(nullable: false),
                        IsRegistered = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ActivationNumber);
            
            CreateTable(
                "dbo.LongProcesses",
                c => new
                    {
                        LongProcessID = c.Int(nullable: false, identity: true),
                        TypeOfProcess = c.Int(nullable: false),
                        Object1ID = c.Int(),
                        Object2ID = c.Int(),
                        Priority = c.Int(nullable: false),
                        AdditionalInfo = c.Binary(),
                        ProgressInfo = c.Int(),
                        Started = c.Boolean(nullable: false),
                        Complete = c.Boolean(nullable: false),
                        ResetWhenComplete = c.Boolean(nullable: false),
                        DelayBeforeRestart = c.Int(),
                        EarliestRestart = c.DateTime(),
                    })
                .PrimaryKey(t => t.LongProcessID);
            
            CreateTable(
                "dbo.RoleStatus",
                c => new
                    {
                        RoleStatusID = c.Int(nullable: false, identity: true),
                        RoleID = c.String(),
                        LastCheckIn = c.DateTime(),
                        IsWorkerRole = c.Boolean(nullable: false),
                        IsBackgroundProcessing = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RoleStatusID);
            
            CreateTable(
                "dbo.UniquenessLockReferences",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UniquenessLockID = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UniquenessLocks", t => t.UniquenessLockID)
                .Index(t => t.UniquenessLockID);
            
            CreateTable(
                "dbo.UniquenessLocks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DeletionTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UniquenessLockReferences", "UniquenessLockID", "dbo.UniquenessLocks");
            DropForeignKey("dbo.TextFields", "FieldID", "dbo.Fields");
            DropForeignKey("dbo.NumberFields", "FieldID", "dbo.Fields");
            DropForeignKey("dbo.DateTimeFields", "FieldID", "dbo.Fields");
            DropForeignKey("dbo.ChoiceFields", "FieldID", "dbo.Fields");
            DropForeignKey("dbo.ChoiceInFields", "ChoiceFieldID", "dbo.ChoiceFields");
            DropForeignKey("dbo.TrustTrackerForChoiceInGroups", "ChoiceInGroupID", "dbo.ChoiceInGroups");
            DropForeignKey("dbo.SearchWordChoices", "ChoiceInGroupID", "dbo.ChoiceInGroups");
            DropForeignKey("dbo.ChoiceInGroups", "ActiveOnDeterminingGroupChoiceInGroupID", "dbo.ChoiceInGroups");
            DropForeignKey("dbo.ChoiceInFields", "ChoiceInGroupID", "dbo.ChoiceInGroups");
            DropForeignKey("dbo.ChoiceInGroups", "ChoiceGroupID", "dbo.ChoiceGroups");
            DropForeignKey("dbo.ChoiceGroups", "DependentOnChoiceGroupID", "dbo.ChoiceGroups");
            DropForeignKey("dbo.ChoiceGroupFieldDefinitions", "ChoiceGroupID", "dbo.ChoiceGroups");
            DropForeignKey("dbo.TextFieldDefinitions", "FieldDefinitionID", "dbo.FieldDefinitions");
            DropForeignKey("dbo.TrustTrackerForChoiceInGroups", "TblID", "dbo.Tbls");
            DropForeignKey("dbo.TblRows", "TblID", "dbo.Tbls");
            DropForeignKey("dbo.FieldDefinitions", "TblID", "dbo.Tbls");
            DropForeignKey("dbo.ChangesGroup", "TblID", "dbo.Tbls");
            DropForeignKey("dbo.UsersAdministrationRightsGroups", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.Tbls", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.ProposalSettings", "TblID", "dbo.Tbls");
            DropForeignKey("dbo.ProposalSettings", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.PointsTotals", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.PointsAdjustments", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.Tbls", "TblDimensionsID", "dbo.TblDimensions");
            DropForeignKey("dbo.Domains", "TblDimensionsID", "dbo.TblDimensions");
            DropForeignKey("dbo.PointsManagers", "DomainID", "dbo.Domains");
            DropForeignKey("dbo.InsertableContents", "TblID", "dbo.Tbls");
            DropForeignKey("dbo.InsertableContents", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.InsertableContents", "DomainID", "dbo.Domains");
            DropForeignKey("dbo.ChoiceGroups", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.ChangesGroup", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.UsersAdministrationRightsGroups", "AdministrationRightsGroupID", "dbo.AdministrationRightsGroups");
            DropForeignKey("dbo.AdministrationRightsGroups", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.AdministrationRights", "AdministrationRightsGroupID", "dbo.AdministrationRightsGroups");
            DropForeignKey("dbo.ProposalEvaluationRatingSettings", "UserActionID", "dbo.UserActions");
            DropForeignKey("dbo.TblColumns", "DefaultRatingGroupAttributesID", "dbo.RatingGroupAttributes");
            DropForeignKey("dbo.RewardRatingSettings", "RatingGroupAttributesID", "dbo.RatingGroupAttributes");
            DropForeignKey("dbo.RewardRatingSettings", "UserActionID", "dbo.UserActions");
            DropForeignKey("dbo.RewardRatingSettings", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.RatingPlans", "OwnedRatingGroupAttributesID", "dbo.RatingGroupAttributes");
            DropForeignKey("dbo.RatingPlans", "RatingGroupAttributesID", "dbo.RatingGroupAttributes");
            DropForeignKey("dbo.RatingGroups", "RatingGroupAttributesID", "dbo.RatingGroupAttributes");
            DropForeignKey("dbo.ProposalEvaluationRatingSettings", "RatingGroupAttributesID", "dbo.RatingGroupAttributes");
            DropForeignKey("dbo.RatingGroupAttributes", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.OverrideCharacteristics", "RatingGroupAttributesID", "dbo.RatingGroupAttributes");
            DropForeignKey("dbo.TblColumns", "TblTabID", "dbo.TblTabs");
            DropForeignKey("dbo.TblTabs", "TblID", "dbo.Tbls");
            DropForeignKey("dbo.TblColumns", "ConditionTblColumnID", "dbo.TblColumns");
            DropForeignKey("dbo.TblColumnFormatting", "TblColumnID", "dbo.TblColumns");
            DropForeignKey("dbo.RatingGroups", "TblColumnID", "dbo.TblColumns");
            DropForeignKey("dbo.VolatilityTrackers", "RatingGroupID", "dbo.RatingGroups");
            DropForeignKey("dbo.UserRatingsToAdd", "TopRatingGroupID", "dbo.RatingGroups");
            DropForeignKey("dbo.UserRatingGroups", "RatingGroupID", "dbo.RatingGroups");
            DropForeignKey("dbo.Ratings", "TopmostRatingGroupID", "dbo.RatingGroups");
            DropForeignKey("dbo.Ratings", "OwnedRatingGroupID", "dbo.RatingGroups");
            DropForeignKey("dbo.Ratings", "RatingGroupID", "dbo.RatingGroups");
            DropForeignKey("dbo.RatingGroupStatusRecords", "RatingGroupID", "dbo.RatingGroups");
            DropForeignKey("dbo.RatingGroupResolutions", "RatingGroupID", "dbo.RatingGroups");
            DropForeignKey("dbo.RatingGroupPhaseStatus", "RatingGroupID", "dbo.RatingGroups");
            DropForeignKey("dbo.UserRatingGroups", "RatingGroupPhaseStatusID", "dbo.RatingGroupPhaseStatus");
            DropForeignKey("dbo.SubsidyAdjustments", "RatingGroupPhaseStatusID", "dbo.RatingGroupPhaseStatus");
            DropForeignKey("dbo.RatingPhaseStatus", "RatingGroupPhaseStatusID", "dbo.RatingGroupPhaseStatus");
            DropForeignKey("dbo.RatingPhases", "RatingPhaseGroupID", "dbo.RatingPhaseGroups");
            DropForeignKey("dbo.RatingGroupPhaseStatus", "RatingPhaseGroupID", "dbo.RatingPhaseGroups");
            DropForeignKey("dbo.RatingCharacteristics", "RatingPhaseGroupID", "dbo.RatingPhaseGroups");
            DropForeignKey("dbo.Ratings", "RatingCharacteristicsID", "dbo.RatingCharacteristics");
            DropForeignKey("dbo.UserRatings", "RatingID", "dbo.Ratings");
            DropForeignKey("dbo.RatingPhaseStatus", "RatingID", "dbo.Ratings");
            DropForeignKey("dbo.UserRatings", "RatingPhaseStatusID", "dbo.RatingPhaseStatus");
            DropForeignKey("dbo.UserRatings", "MostRecentUserRatingID", "dbo.UserRatings");
            DropForeignKey("dbo.UserRatings", "UserRatingGroupID", "dbo.UserRatingGroups");
            DropForeignKey("dbo.TrustTrackerForChoiceInGroupsUserRatingLinks", "UserRatingID", "dbo.UserRatings");
            DropForeignKey("dbo.UserRatings", "RewardPendingPointsTrackerID", "dbo.RewardPendingPointsTrackers");
            DropForeignKey("dbo.VolatilityTblRowTrackers", "TblRowID", "dbo.TblRows");
            DropForeignKey("dbo.VolatilityTrackers", "VolatilityTblRowTrackerID", "dbo.VolatilityTblRowTrackers");
            DropForeignKey("dbo.TblRowStatusRecord", "TblRowId", "dbo.TblRows");
            DropForeignKey("dbo.TblRows", "TblRowFieldDisplayID", "dbo.TblRowFieldDisplays");
            DropForeignKey("dbo.SearchWordTblRowNames", "TblRowID", "dbo.TblRows");
            DropForeignKey("dbo.SearchWordTextFields", "SearchWordID", "dbo.SearchWords");
            DropForeignKey("dbo.SearchWordTextFields", "TextFieldID", "dbo.TextFields");
            DropForeignKey("dbo.SearchWordTblRowNames", "SearchWordID", "dbo.SearchWords");
            DropForeignKey("dbo.SearchWordHierarchyItems", "SearchWordID", "dbo.SearchWords");
            DropForeignKey("dbo.HierarchyItems", "TblID", "dbo.Tbls");
            DropForeignKey("dbo.SearchWordHierarchyItems", "HierarchyItemID", "dbo.HierarchyItems");
            DropForeignKey("dbo.HierarchyItems", "HigherHierarchyItemID", "dbo.HierarchyItems");
            DropForeignKey("dbo.HierarchyItems", "HigherHierarchyItemForRoutingID", "dbo.HierarchyItems");
            DropForeignKey("dbo.SearchWordChoices", "SearchWordID", "dbo.SearchWords");
            DropForeignKey("dbo.RewardPendingPointsTrackers", "RewardTblRowID", "dbo.TblRows");
            DropForeignKey("dbo.RatingGroups", "TblRowID", "dbo.TblRows");
            DropForeignKey("dbo.OverrideCharacteristics", "TblRowID", "dbo.TblRows");
            DropForeignKey("dbo.Fields", "TblRowID", "dbo.TblRows");
            DropForeignKey("dbo.Comments", "TblRowID", "dbo.TblRows");
            DropForeignKey("dbo.UsersRights", "UserID", "dbo.Users");
            DropForeignKey("dbo.UsersRights", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.UsersAdministrationRightsGroups", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserRatingsToAdd", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserRatings", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserInteractions", "LatestRatingUserID", "dbo.Users");
            DropForeignKey("dbo.UserInteractions", "OrigRatingUserID", "dbo.Users");
            DropForeignKey("dbo.UserInfo", "UserInfoID", "dbo.Users");
            DropForeignKey("dbo.UserCheckIns", "UserID", "dbo.Users");
            DropForeignKey("dbo.TrustTrackers", "UserID", "dbo.Users");
            DropForeignKey("dbo.TrustTrackerStats", "TrustTrackerID", "dbo.TrustTrackers");
            DropForeignKey("dbo.UserInteractionStats", "TrustTrackerStatID", "dbo.TrustTrackerStats");
            DropForeignKey("dbo.UserInteractionStats", "UserInteractionID", "dbo.UserInteractions");
            DropForeignKey("dbo.UserRatings", "TrustTrackerUnitID", "dbo.TrustTrackerUnits");
            DropForeignKey("dbo.UserInteractions", "TrustTrackerUnitID", "dbo.TrustTrackerUnits");
            DropForeignKey("dbo.TrustTrackers", "TrustTrackerUnitID", "dbo.TrustTrackerUnits");
            DropForeignKey("dbo.TblColumns", "TrustTrackerUnitID", "dbo.TrustTrackerUnits");
            DropForeignKey("dbo.PointsManagers", "TrustTrackerUnitID", "dbo.TrustTrackerUnits");
            DropForeignKey("dbo.TrustTrackerForChoiceInGroups", "UserID", "dbo.Users");
            DropForeignKey("dbo.TrustTrackerForChoiceInGroupsUserRatingLinks", "TrustTrackerForChoiceInGroupID", "dbo.TrustTrackerForChoiceInGroups");
            DropForeignKey("dbo.Tbls", "Creator", "dbo.Users");
            DropForeignKey("dbo.SubsidyDensityRangeGroups", "Creator", "dbo.Users");
            DropForeignKey("dbo.SubsidyDensityRanges", "SubsidyDensityRangeGroupID", "dbo.SubsidyDensityRangeGroups");
            DropForeignKey("dbo.RatingCharacteristics", "SubsidyDensityRangeGroupID", "dbo.SubsidyDensityRangeGroups");
            DropForeignKey("dbo.RewardPendingPointsTrackers", "UserID", "dbo.Users");
            DropForeignKey("dbo.Ratings", "Creator", "dbo.Users");
            DropForeignKey("dbo.RatingPlans", "Creator", "dbo.Users");
            DropForeignKey("dbo.RatingPhaseGroups", "Creator", "dbo.Users");
            DropForeignKey("dbo.RatingGroupResolutions", "Creator", "dbo.Users");
            DropForeignKey("dbo.RatingCharacteristics", "Creator", "dbo.Users");
            DropForeignKey("dbo.PointsTotals", "UserID", "dbo.Users");
            DropForeignKey("dbo.PointsAdjustments", "UserID", "dbo.Users");
            DropForeignKey("dbo.Comments", "UserID", "dbo.Users");
            DropForeignKey("dbo.ChoiceGroups", "Creator", "dbo.Users");
            DropForeignKey("dbo.ChangesGroup", "Creator", "dbo.Users");
            DropForeignKey("dbo.Ratings", "MostRecentUserRatingID", "dbo.UserRatings");
            DropForeignKey("dbo.RatingConditions", "ConditionRatingID", "dbo.Ratings");
            DropForeignKey("dbo.RatingGroupAttributes", "RatingConditionID", "dbo.RatingConditions");
            DropForeignKey("dbo.ChangesGroup", "RewardRatingID", "dbo.Ratings");
            DropForeignKey("dbo.RatingGroupAttributes", "RatingCharacteristicsID", "dbo.RatingCharacteristics");
            DropForeignKey("dbo.RatingGroupPhaseStatus", "RatingPhaseID", "dbo.RatingPhases");
            DropForeignKey("dbo.OverrideCharacteristics", "TblColumnID", "dbo.TblColumns");
            DropForeignKey("dbo.ProposalEvaluationRatingSettings", "PointsManagerID", "dbo.PointsManagers");
            DropForeignKey("dbo.AdministrationRights", "UserActionID", "dbo.UserActions");
            DropForeignKey("dbo.ChangesStatusOfObject", "ChangesGroupID", "dbo.ChangesGroup");
            DropForeignKey("dbo.NumberFieldDefinitions", "FieldDefinitionID", "dbo.FieldDefinitions");
            DropForeignKey("dbo.Fields", "FieldDefinitionID", "dbo.FieldDefinitions");
            DropForeignKey("dbo.DateTimeFieldDefinitions", "FieldDefinitionID", "dbo.FieldDefinitions");
            DropForeignKey("dbo.ChoiceGroupFieldDefinitions", "FieldDefinitionID", "dbo.FieldDefinitions");
            DropForeignKey("dbo.ChoiceGroupFieldDefinitions", "DependentOnChoiceGroupFieldDefinitionID", "dbo.ChoiceGroupFieldDefinitions");
            DropForeignKey("dbo.AddressFields", "FieldID", "dbo.Fields");
            DropIndex("dbo.UniquenessLockReferences", new[] { "UniquenessLockID" });
            DropIndex("dbo.NumberFields", new[] { "FieldID" });
            DropIndex("dbo.DateTimeFields", new[] { "FieldID" });
            DropIndex("dbo.TextFieldDefinitions", new[] { "FieldDefinitionID" });
            DropIndex("dbo.ProposalSettings", new[] { "TblID" });
            DropIndex("dbo.ProposalSettings", new[] { "PointsManagerID" });
            DropIndex("dbo.InsertableContents", new[] { "TblID" });
            DropIndex("dbo.InsertableContents", new[] { "PointsManagerID" });
            DropIndex("dbo.InsertableContents", new[] { "DomainID" });
            DropIndex("dbo.Domains", new[] { "TblDimensionsID" });
            DropIndex("dbo.RewardRatingSettings", new[] { "RatingGroupAttributesID" });
            DropIndex("dbo.RewardRatingSettings", new[] { "UserActionID" });
            DropIndex("dbo.RewardRatingSettings", new[] { "PointsManagerID" });
            DropIndex("dbo.TblTabs", new[] { "TblID" });
            DropIndex("dbo.TblColumnFormatting", new[] { "TblColumnID" });
            DropIndex("dbo.RatingGroupStatusRecords", new[] { "RatingGroupID" });
            DropIndex("dbo.SubsidyAdjustments", new[] { "RatingGroupPhaseStatusID" });
            DropIndex("dbo.UserRatingGroups", new[] { "RatingGroupPhaseStatusID" });
            DropIndex("dbo.UserRatingGroups", new[] { "RatingGroupID" });
            DropIndex("dbo.VolatilityTrackers", new[] { "VolatilityTblRowTrackerID" });
            DropIndex("dbo.VolatilityTrackers", new[] { "RatingGroupID" });
            DropIndex("dbo.VolatilityTblRowTrackers", new[] { "TblRowID" });
            DropIndex("dbo.TblRowStatusRecord", new[] { "TblRowId" });
            DropIndex("dbo.TextFields", new[] { "FieldID" });
            DropIndex("dbo.SearchWordTextFields", new[] { "SearchWordID" });
            DropIndex("dbo.SearchWordTextFields", new[] { "TextFieldID" });
            DropIndex("dbo.HierarchyItems", new[] { "TblID" });
            DropIndex("dbo.HierarchyItems", new[] { "HigherHierarchyItemForRoutingID" });
            DropIndex("dbo.HierarchyItems", new[] { "HigherHierarchyItemID" });
            DropIndex("dbo.SearchWordHierarchyItems", new[] { "SearchWordID" });
            DropIndex("dbo.SearchWordHierarchyItems", new[] { "HierarchyItemID" });
            DropIndex("dbo.SearchWordChoices", new[] { "SearchWordID" });
            DropIndex("dbo.SearchWordChoices", new[] { "ChoiceInGroupID" });
            DropIndex("dbo.SearchWordTblRowNames", new[] { "SearchWordID" });
            DropIndex("dbo.SearchWordTblRowNames", new[] { "TblRowID" });
            DropIndex("dbo.UsersRights", new[] { "PointsManagerID" });
            DropIndex("dbo.UsersRights", new[] { "UserID" });
            DropIndex("dbo.UsersAdministrationRightsGroups", new[] { "AdministrationRightsGroupID" });
            DropIndex("dbo.UsersAdministrationRightsGroups", new[] { "PointsManagerID" });
            DropIndex("dbo.UsersAdministrationRightsGroups", new[] { "UserID" });
            DropIndex("dbo.UserRatingsToAdd", new[] { "TopRatingGroupID" });
            DropIndex("dbo.UserRatingsToAdd", new[] { "UserID" });
            DropIndex("dbo.UserInfo", new[] { "UserInfoID" });
            DropIndex("dbo.UserCheckIns", new[] { "UserID" });
            DropIndex("dbo.UserInteractions", new[] { "LatestRatingUserID" });
            DropIndex("dbo.UserInteractions", new[] { "OrigRatingUserID" });
            DropIndex("dbo.UserInteractions", new[] { "TrustTrackerUnitID" });
            DropIndex("dbo.UserInteractionStats", new[] { "TrustTrackerStatID" });
            DropIndex("dbo.UserInteractionStats", new[] { "UserInteractionID" });
            DropIndex("dbo.TrustTrackerStats", new[] { "TrustTrackerID" });
            DropIndex("dbo.TrustTrackers", new[] { "UserID" });
            DropIndex("dbo.TrustTrackers", new[] { "TrustTrackerUnitID" });
            DropIndex("dbo.TrustTrackerForChoiceInGroupsUserRatingLinks", new[] { "TrustTrackerForChoiceInGroupID" });
            DropIndex("dbo.TrustTrackerForChoiceInGroupsUserRatingLinks", new[] { "UserRatingID" });
            DropIndex("dbo.TrustTrackerForChoiceInGroups", new[] { "TblID" });
            DropIndex("dbo.TrustTrackerForChoiceInGroups", new[] { "ChoiceInGroupID" });
            DropIndex("dbo.TrustTrackerForChoiceInGroups", new[] { "UserID" });
            DropIndex("dbo.SubsidyDensityRanges", new[] { "SubsidyDensityRangeGroupID" });
            DropIndex("dbo.SubsidyDensityRangeGroups", new[] { "Creator" });
            DropIndex("dbo.RatingPlans", new[] { "Creator" });
            DropIndex("dbo.RatingPlans", new[] { "OwnedRatingGroupAttributesID" });
            DropIndex("dbo.RatingPlans", new[] { "RatingGroupAttributesID" });
            DropIndex("dbo.RatingGroupResolutions", new[] { "Creator" });
            DropIndex("dbo.RatingGroupResolutions", new[] { "RatingGroupID" });
            DropIndex("dbo.PointsTotals", new[] { "PointsManagerID" });
            DropIndex("dbo.PointsTotals", new[] { "UserID" });
            DropIndex("dbo.PointsAdjustments", new[] { "PointsManagerID" });
            DropIndex("dbo.PointsAdjustments", new[] { "UserID" });
            DropIndex("dbo.Comments", new[] { "UserID" });
            DropIndex("dbo.Comments", new[] { "TblRowID" });
            DropIndex("dbo.TblRows", new[] { "TblRowFieldDisplayID" });
            DropIndex("dbo.TblRows", new[] { "TblID" });
            DropIndex("dbo.RewardPendingPointsTrackers", new[] { "UserID" });
            DropIndex("dbo.RewardPendingPointsTrackers", new[] { "RewardTblRowID" });
            DropIndex("dbo.UserRatings", new[] { "MostRecentUserRatingID" });
            DropIndex("dbo.UserRatings", new[] { "RewardPendingPointsTrackerID" });
            DropIndex("dbo.UserRatings", new[] { "TrustTrackerUnitID" });
            DropIndex("dbo.UserRatings", new[] { "UserID" });
            DropIndex("dbo.UserRatings", new[] { "RatingPhaseStatusID" });
            DropIndex("dbo.UserRatings", new[] { "RatingID" });
            DropIndex("dbo.UserRatings", new[] { "UserRatingGroupID" });
            DropIndex("dbo.RatingPhaseStatus", new[] { "RatingID" });
            DropIndex("dbo.RatingPhaseStatus", new[] { "RatingGroupPhaseStatusID" });
            DropIndex("dbo.RatingConditions", new[] { "ConditionRatingID" });
            DropIndex("dbo.Ratings", new[] { "Creator" });
            DropIndex("dbo.Ratings", new[] { "MostRecentUserRatingID" });
            DropIndex("dbo.Ratings", new[] { "TopmostRatingGroupID" });
            DropIndex("dbo.Ratings", new[] { "OwnedRatingGroupID" });
            DropIndex("dbo.Ratings", new[] { "RatingCharacteristicsID" });
            DropIndex("dbo.Ratings", new[] { "RatingGroupID" });
            DropIndex("dbo.RatingCharacteristics", new[] { "Creator" });
            DropIndex("dbo.RatingCharacteristics", new[] { "SubsidyDensityRangeGroupID" });
            DropIndex("dbo.RatingCharacteristics", new[] { "RatingPhaseGroupID" });
            DropIndex("dbo.RatingPhaseGroups", new[] { "Creator" });
            DropIndex("dbo.RatingPhases", new[] { "RatingPhaseGroupID" });
            DropIndex("dbo.RatingGroupPhaseStatus", new[] { "RatingGroupID" });
            DropIndex("dbo.RatingGroupPhaseStatus", new[] { "RatingPhaseID" });
            DropIndex("dbo.RatingGroupPhaseStatus", new[] { "RatingPhaseGroupID" });
            DropIndex("dbo.RatingGroups", new[] { "TblColumnID" });
            DropIndex("dbo.RatingGroups", new[] { "TblRowID" });
            DropIndex("dbo.RatingGroups", new[] { "RatingGroupAttributesID" });
            DropIndex("dbo.TblColumns", new[] { "TrustTrackerUnitID" });
            DropIndex("dbo.TblColumns", new[] { "ConditionTblColumnID" });
            DropIndex("dbo.TblColumns", new[] { "DefaultRatingGroupAttributesID" });
            DropIndex("dbo.TblColumns", new[] { "TblTabID" });
            DropIndex("dbo.OverrideCharacteristics", new[] { "TblColumnID" });
            DropIndex("dbo.OverrideCharacteristics", new[] { "TblRowID" });
            DropIndex("dbo.OverrideCharacteristics", new[] { "RatingGroupAttributesID" });
            DropIndex("dbo.RatingGroupAttributes", new[] { "PointsManagerID" });
            DropIndex("dbo.RatingGroupAttributes", new[] { "RatingConditionID" });
            DropIndex("dbo.RatingGroupAttributes", new[] { "RatingCharacteristicsID" });
            DropIndex("dbo.ProposalEvaluationRatingSettings", new[] { "RatingGroupAttributesID" });
            DropIndex("dbo.ProposalEvaluationRatingSettings", new[] { "UserActionID" });
            DropIndex("dbo.ProposalEvaluationRatingSettings", new[] { "PointsManagerID" });
            DropIndex("dbo.AdministrationRights", new[] { "UserActionID" });
            DropIndex("dbo.AdministrationRights", new[] { "AdministrationRightsGroupID" });
            DropIndex("dbo.AdministrationRightsGroups", new[] { "PointsManagerID" });
            DropIndex("dbo.PointsManagers", new[] { "TrustTrackerUnitID" });
            DropIndex("dbo.PointsManagers", new[] { "DomainID" });
            DropIndex("dbo.ChangesStatusOfObject", new[] { "ChangesGroupID" });
            DropIndex("dbo.ChangesGroup", new[] { "RewardRatingID" });
            DropIndex("dbo.ChangesGroup", new[] { "Creator" });
            DropIndex("dbo.ChangesGroup", new[] { "TblID" });
            DropIndex("dbo.ChangesGroup", new[] { "PointsManagerID" });
            DropIndex("dbo.Tbls", new[] { "Creator" });
            DropIndex("dbo.Tbls", new[] { "TblDimensionsID" });
            DropIndex("dbo.Tbls", new[] { "PointsManagerID" });
            DropIndex("dbo.NumberFieldDefinitions", new[] { "FieldDefinitionID" });
            DropIndex("dbo.DateTimeFieldDefinitions", new[] { "FieldDefinitionID" });
            DropIndex("dbo.FieldDefinitions", new[] { "TblID" });
            DropIndex("dbo.ChoiceGroupFieldDefinitions", new[] { "DependentOnChoiceGroupFieldDefinitionID" });
            DropIndex("dbo.ChoiceGroupFieldDefinitions", new[] { "FieldDefinitionID" });
            DropIndex("dbo.ChoiceGroupFieldDefinitions", new[] { "ChoiceGroupID" });
            DropIndex("dbo.ChoiceGroups", new[] { "Creator" });
            DropIndex("dbo.ChoiceGroups", new[] { "DependentOnChoiceGroupID" });
            DropIndex("dbo.ChoiceGroups", new[] { "PointsManagerID" });
            DropIndex("dbo.ChoiceInGroups", new[] { "ActiveOnDeterminingGroupChoiceInGroupID" });
            DropIndex("dbo.ChoiceInGroups", new[] { "ChoiceGroupID" });
            DropIndex("dbo.ChoiceInFields", new[] { "ChoiceInGroupID" });
            DropIndex("dbo.ChoiceInFields", new[] { "ChoiceFieldID" });
            DropIndex("dbo.ChoiceFields", new[] { "FieldID" });
            DropIndex("dbo.Fields", new[] { "FieldDefinitionID" });
            DropIndex("dbo.Fields", new[] { "TblRowID" });
            DropIndex("dbo.AddressFields", new[] { "FieldID" });
            DropTable("dbo.UniquenessLocks");
            DropTable("dbo.UniquenessLockReferences");
            DropTable("dbo.RoleStatus");
            DropTable("dbo.LongProcesses");
            DropTable("dbo.InvitedUser");
            DropTable("dbo.DatabaseStatus");
            DropTable("dbo.NumberFields");
            DropTable("dbo.DateTimeFields");
            DropTable("dbo.TextFieldDefinitions");
            DropTable("dbo.ProposalSettings");
            DropTable("dbo.TblDimensions");
            DropTable("dbo.InsertableContents");
            DropTable("dbo.Domains");
            DropTable("dbo.RewardRatingSettings");
            DropTable("dbo.TblTabs");
            DropTable("dbo.TblColumnFormatting");
            DropTable("dbo.RatingGroupStatusRecords");
            DropTable("dbo.SubsidyAdjustments");
            DropTable("dbo.UserRatingGroups");
            DropTable("dbo.VolatilityTrackers");
            DropTable("dbo.VolatilityTblRowTrackers");
            DropTable("dbo.TblRowStatusRecord");
            DropTable("dbo.TblRowFieldDisplays");
            DropTable("dbo.TextFields");
            DropTable("dbo.SearchWordTextFields");
            DropTable("dbo.HierarchyItems");
            DropTable("dbo.SearchWordHierarchyItems");
            DropTable("dbo.SearchWordChoices");
            DropTable("dbo.SearchWords");
            DropTable("dbo.SearchWordTblRowNames");
            DropTable("dbo.UsersRights");
            DropTable("dbo.UsersAdministrationRightsGroups");
            DropTable("dbo.UserRatingsToAdd");
            DropTable("dbo.UserInfo");
            DropTable("dbo.UserCheckIns");
            DropTable("dbo.TrustTrackerUnits");
            DropTable("dbo.UserInteractions");
            DropTable("dbo.UserInteractionStats");
            DropTable("dbo.TrustTrackerStats");
            DropTable("dbo.TrustTrackers");
            DropTable("dbo.TrustTrackerForChoiceInGroupsUserRatingLinks");
            DropTable("dbo.TrustTrackerForChoiceInGroups");
            DropTable("dbo.SubsidyDensityRanges");
            DropTable("dbo.SubsidyDensityRangeGroups");
            DropTable("dbo.RatingPlans");
            DropTable("dbo.RatingGroupResolutions");
            DropTable("dbo.PointsTotals");
            DropTable("dbo.PointsAdjustments");
            DropTable("dbo.Users");
            DropTable("dbo.Comments");
            DropTable("dbo.TblRows");
            DropTable("dbo.RewardPendingPointsTrackers");
            DropTable("dbo.UserRatings");
            DropTable("dbo.RatingPhaseStatus");
            DropTable("dbo.RatingConditions");
            DropTable("dbo.Ratings");
            DropTable("dbo.RatingCharacteristics");
            DropTable("dbo.RatingPhaseGroups");
            DropTable("dbo.RatingPhases");
            DropTable("dbo.RatingGroupPhaseStatus");
            DropTable("dbo.RatingGroups");
            DropTable("dbo.TblColumns");
            DropTable("dbo.OverrideCharacteristics");
            DropTable("dbo.RatingGroupAttributes");
            DropTable("dbo.ProposalEvaluationRatingSettings");
            DropTable("dbo.UserActions");
            DropTable("dbo.AdministrationRights");
            DropTable("dbo.AdministrationRightsGroups");
            DropTable("dbo.PointsManagers");
            DropTable("dbo.ChangesStatusOfObject");
            DropTable("dbo.ChangesGroup");
            DropTable("dbo.Tbls");
            DropTable("dbo.NumberFieldDefinitions");
            DropTable("dbo.DateTimeFieldDefinitions");
            DropTable("dbo.FieldDefinitions");
            DropTable("dbo.ChoiceGroupFieldDefinitions");
            DropTable("dbo.ChoiceGroups");
            DropTable("dbo.ChoiceInGroups");
            DropTable("dbo.ChoiceInFields");
            DropTable("dbo.ChoiceFields");
            DropTable("dbo.Fields");
            DropTable("dbo.AddressFields");
        }
    }
}
