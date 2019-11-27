namespace CSCZJ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GovernmentUnit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        GovernmentType = c.Int(nullable: false),
                        IsChargeDepartment = c.Boolean(nullable: false),
                        Address = c.String(),
                        Person = c.String(maxLength: 255),
                        Tel = c.String(maxLength: 255),
                        PeopleCount = c.Int(nullable: false),
                        RealPeopleCount = c.Int(nullable: false),
                        Location = c.Geography(),
                        LandOrigin = c.String(),
                        LandArea = c.Double(),
                        ConstructArea = c.Double(),
                        OfficeArea = c.Double(),
                        Floor = c.Int(),
                        HasLandCard = c.Boolean(nullable: false),
                        HasConstructCard = c.Boolean(nullable: false),
                        HasRentInCard = c.Boolean(nullable: false),
                        HasRentCard = c.Boolean(nullable: false),
                        HasLendInCard = c.Boolean(nullable: false),
                        Remark = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        X = c.Double(),
                        Y = c.Double(),
                        RentArea = c.Single(),
                        RentPart = c.String(),
                        RentInfo = c.String(),
                        ParentGovernmentId = c.Int(nullable: false),
                        CreditCode = c.String(),
                        PersonNumber = c.Int(nullable: false),
                        PropertyConut = c.Int(nullable: false),
                        ParentName = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Property",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        PropertyType = c.Int(nullable: false),
                        Region = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Address = c.String(),
                        ConstructArea = c.Double(nullable: false),
                        LandArea = c.Double(nullable: false),
                        Floor = c.Int(nullable: false),
                        PropertyID = c.String(),
                        Account = c.Boolean(nullable: false),
                        GetedDate = c.DateTime(),
                        UsedPeople = c.String(nullable: false, maxLength: 255),
                        Location = c.Geography(),
                        X = c.Double(nullable: false),
                        Y = c.Double(nullable: false),
                        Extent = c.Geography(),
                        Description = c.String(),
                        FourToStation = c.String(),
                        Published = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Error = c.String(),
                        WKT = c.String(),
                        EstateId = c.String(),
                        ConstructId = c.String(),
                        LandId = c.String(),
                        CurrentType = c.Int(nullable: false),
                        UserType = c.Int(nullable: false),
                        Mortgage = c.Boolean(nullable: false),
                        Locked = c.Boolean(nullable: false),
                        Off = c.Boolean(nullable: false),
                        FromExcelImport = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        Government_Id = c.Int(),
                        PropertyNewCreate_Id = c.Int(),
                        PropertyOff_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GovernmentUnit", t => t.Government_Id)
                .ForeignKey("dbo.PropertyNewCreate", t => t.PropertyNewCreate_Id)
                .ForeignKey("dbo.PropertyOff", t => t.PropertyOff_Id)
                .Index(t => t.Government_Id)
                .Index(t => t.PropertyNewCreate_Id)
                .Index(t => t.PropertyOff_Id);
            
            CreateTable(
                "dbo.PropertyAllot",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessDate = c.DateTime(nullable: false),
                        Title = c.String(),
                        PrePropertyOwner = c.String(),
                        NowPropertyOwner = c.String(),
                        NowGovernmentId = c.Int(nullable: false),
                        AllotTime = c.DateTime(nullable: false),
                        Remark = c.String(),
                        DSuggestion = c.String(),
                        ASuggestion = c.String(),
                        State = c.Int(nullable: false),
                        DApproveDate = c.DateTime(),
                        AApproveDate = c.DateTime(),
                        SuggestGovernmentId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        Property_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Property", t => t.Property_Id, cascadeDelete: true)
                .Index(t => t.Property_Id);
            
            CreateTable(
                "dbo.PropertyAllot_File_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyAllotId = c.Int(nullable: false),
                        FileId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.PropertyAllot", t => t.PropertyAllotId, cascadeDelete: true)
                .Index(t => t.PropertyAllotId)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Extension = c.String(),
                        SeoFilename = c.String(),
                        IsNew = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PropertyAllot_Picture_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyAllotId = c.Int(nullable: false),
                        PictureId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pictures", t => t.PictureId, cascadeDelete: true)
                .ForeignKey("dbo.PropertyAllot", t => t.PropertyAllotId, cascadeDelete: true)
                .Index(t => t.PropertyAllotId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PictureBinary = c.Binary(),
                        MimeType = c.String(),
                        SeoFilename = c.String(),
                        AltAttribute = c.String(),
                        TitleAttribute = c.String(),
                        IsNew = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PropertyEdit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessDate = c.DateTime(nullable: false),
                        Title = c.String(),
                        DSuggestion = c.String(),
                        DApproveDate = c.DateTime(),
                        ASuggestion = c.String(),
                        AApproveDate = c.DateTime(),
                        State = c.Int(nullable: false),
                        SuggestGovernmentId = c.Int(nullable: false),
                        CopyProperty_Id = c.Int(nullable: false),
                        OriginCopyProperty_Id = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        Property_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Property", t => t.Property_Id, cascadeDelete: true)
                .Index(t => t.Property_Id);
            
            CreateTable(
                "dbo.Property_File_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        FileId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .Index(t => t.PropertyId)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.PropertyLend",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessDate = c.DateTime(nullable: false),
                        Name = c.String(),
                        Title = c.String(),
                        LendTime = c.DateTime(nullable: false),
                        LendArea = c.Double(nullable: false),
                        BackTime = c.DateTime(),
                        Remark = c.String(),
                        DSuggestion = c.String(),
                        DApproveDate = c.DateTime(),
                        ASuggestion = c.String(),
                        AApproveDate = c.DateTime(),
                        State = c.Int(nullable: false),
                        SuggestGovernmentId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        Property_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Property", t => t.Property_Id, cascadeDelete: true)
                .Index(t => t.Property_Id);
            
            CreateTable(
                "dbo.PropertyLend_File_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyLendId = c.Int(nullable: false),
                        FileId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.PropertyLend", t => t.PropertyLendId, cascadeDelete: true)
                .Index(t => t.PropertyLendId)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.PropertyLend_Picture_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyLendId = c.Int(nullable: false),
                        PictureId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pictures", t => t.PictureId, cascadeDelete: true)
                .ForeignKey("dbo.PropertyLend", t => t.PropertyLendId, cascadeDelete: true)
                .Index(t => t.PropertyLendId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.Property_Picture_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        PictureId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        IsLogo = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pictures", t => t.PictureId, cascadeDelete: true)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .Index(t => t.PropertyId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.PropertyNewCreate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessDate = c.DateTime(nullable: false),
                        Title = c.String(),
                        DSuggestion = c.String(),
                        DApproveDate = c.DateTime(),
                        ASuggestion = c.String(),
                        AApproveDate = c.DateTime(),
                        State = c.Int(nullable: false),
                        SuggestGovernmentId = c.Int(nullable: false),
                        Property_Id = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Property", t => t.Property_Id, cascadeDelete: true)
                .Index(t => t.Property_Id);
            
            CreateTable(
                "dbo.PropertyOff",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessDate = c.DateTime(nullable: false),
                        Title = c.String(),
                        OffTime = c.DateTime(nullable: false),
                        Reason = c.String(),
                        Price = c.Single(nullable: false),
                        Remark = c.String(),
                        DSuggestion = c.String(),
                        DApproveDate = c.DateTime(),
                        ASuggestion = c.String(),
                        AApproveDate = c.DateTime(),
                        OffType = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        Property_Id = c.Int(nullable: false),
                        SuggestGovernmentId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Property", t => t.Property_Id, cascadeDelete: true)
                .Index(t => t.Property_Id);
            
            CreateTable(
                "dbo.PropertyOff_File_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyOffId = c.Int(nullable: false),
                        FileId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.PropertyOff", t => t.PropertyOffId, cascadeDelete: true)
                .Index(t => t.PropertyOffId)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.PropertyOff_Picture_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyOffId = c.Int(nullable: false),
                        PictureId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pictures", t => t.PictureId, cascadeDelete: true)
                .ForeignKey("dbo.PropertyOff", t => t.PropertyOffId, cascadeDelete: true)
                .Index(t => t.PropertyOffId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.PropertyRent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessDate = c.DateTime(nullable: false),
                        Name = c.String(),
                        Title = c.String(),
                        RentTime = c.DateTime(nullable: false),
                        BackTime = c.DateTime(nullable: false),
                        PriceString = c.String(),
                        RentArea = c.Double(nullable: false),
                        RentMonth = c.Int(nullable: false),
                        RentPrice = c.Single(nullable: false),
                        Remark = c.String(),
                        DSuggestion = c.String(),
                        DApproveDate = c.DateTime(),
                        ASuggestion = c.String(),
                        AApproveDate = c.DateTime(),
                        State = c.Int(nullable: false),
                        SuggestGovernmentId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        Property_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Property", t => t.Property_Id, cascadeDelete: true)
                .Index(t => t.Property_Id);
            
            CreateTable(
                "dbo.PropertyRent_File_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyRentId = c.Int(nullable: false),
                        FileId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.PropertyRent", t => t.PropertyRentId, cascadeDelete: true)
                .Index(t => t.PropertyRentId)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.PropertyRent_Picture_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyRentId = c.Int(nullable: false),
                        PictureId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pictures", t => t.PictureId, cascadeDelete: true)
                .ForeignKey("dbo.PropertyRent", t => t.PropertyRentId, cascadeDelete: true)
                .Index(t => t.PropertyRentId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.AccountUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 1000),
                        NickName = c.String(),
                        Active = c.Boolean(nullable: false),
                        AccountUserGuid = c.Guid(nullable: false),
                        LastIpAddress = c.String(),
                        LastActivityDate = c.DateTime(),
                        LastLoginDate = c.DateTime(),
                        Password = c.String(),
                        PasswordFormatId = c.Int(nullable: false),
                        PasswordSalt = c.String(),
                        IsSystemAccount = c.Boolean(nullable: false),
                        SystemName = c.String(maxLength: 400),
                        DisplayOrder = c.Int(nullable: false),
                        Remark = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        Government_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GovernmentUnit", t => t.Government_Id)
                .Index(t => t.Government_Id);
            
            CreateTable(
                "dbo.AccountUserRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Active = c.Boolean(nullable: false),
                        IsSystemRole = c.Boolean(nullable: false),
                        SystemName = c.String(maxLength: 255),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MonthTotal",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Property_ID = c.Int(nullable: false),
                        Property_Name = c.String(),
                        CurrentUse_Self = c.Double(nullable: false),
                        CurrentUse_Rent = c.Double(nullable: false),
                        CurrentUse_Lend = c.Double(nullable: false),
                        CurrentUse_Idle = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Month = c.DateTime(nullable: false),
                        Income = c.Double(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CopyProperty",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        PropertyType = c.Int(nullable: false),
                        Region = c.Int(nullable: false),
                        Address = c.String(),
                        ConstructArea = c.Double(nullable: false),
                        LandArea = c.Double(nullable: false),
                        PropertyID = c.String(),
                        HasConstructID = c.Boolean(nullable: false),
                        HasLandID = c.Boolean(nullable: false),
                        PropertyNature = c.String(),
                        LandNature = c.String(),
                        Price = c.Double(nullable: false),
                        GetedDate = c.DateTime(),
                        LifeTime = c.Int(nullable: false),
                        UsedPeople = c.String(nullable: false, maxLength: 255),
                        CurrentUse_Self = c.Double(nullable: false),
                        CurrentUse_Rent = c.Double(nullable: false),
                        CurrentUse_Lend = c.Double(nullable: false),
                        CurrentUse_Idle = c.Double(nullable: false),
                        NextStepUsage = c.Int(nullable: false),
                        Location = c.String(),
                        Extent = c.String(),
                        Description = c.String(),
                        Published = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Government_Id = c.Int(nullable: false),
                        EstateId = c.String(),
                        ConstructId = c.String(),
                        LandId = c.String(),
                        Locked = c.Boolean(nullable: false),
                        Off = c.Boolean(nullable: false),
                        Property_Id = c.Int(nullable: false),
                        PrictureIds = c.String(),
                        FileIds = c.String(),
                        LogoPicture_Id = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubmitRecord",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Goverment_ID = c.Int(nullable: false),
                        RecordDate = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 255),
                        DisplayName = c.String(maxLength: 255),
                        Host = c.String(nullable: false, maxLength: 255),
                        Port = c.Int(nullable: false),
                        Username = c.String(nullable: false, maxLength: 255),
                        Password = c.String(nullable: false, maxLength: 255),
                        EnableSsl = c.Boolean(nullable: false),
                        UseDefaultCredentials = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MessageTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        BccEmailAddresses = c.String(maxLength: 200),
                        Subject = c.String(maxLength: 1000),
                        Body = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AttachedDownloadId = c.Int(nullable: false),
                        EmailAccountId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QueuedEmail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PriorityId = c.Int(nullable: false),
                        From = c.String(nullable: false, maxLength: 500),
                        FromName = c.String(maxLength: 500),
                        To = c.String(nullable: false, maxLength: 500),
                        ToName = c.String(maxLength: 500),
                        ReplyTo = c.String(maxLength: 500),
                        ReplyToName = c.String(maxLength: 500),
                        CC = c.String(maxLength: 500),
                        Bcc = c.String(maxLength: 500),
                        Subject = c.String(maxLength: 1000),
                        Body = c.String(),
                        AttachmentFilePath = c.String(),
                        AttachmentFileName = c.String(),
                        AttachedDownloadId = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        SentTries = c.Int(nullable: false),
                        SentOnUtc = c.DateTime(),
                        EmailAccountId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailAccount", t => t.EmailAccountId, cascadeDelete: true)
                .Index(t => t.EmailAccountId);
            
            CreateTable(
                "dbo.ActivityLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActivityLogTypeId = c.Int(nullable: false),
                        AccountUserId = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountUser", t => t.AccountUserId, cascadeDelete: true)
                .ForeignKey("dbo.ActivityLogType", t => t.ActivityLogTypeId, cascadeDelete: true)
                .Index(t => t.ActivityLogTypeId)
                .Index(t => t.AccountUserId);
            
            CreateTable(
                "dbo.ActivityLogType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemKeyword = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 200),
                        Enabled = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Log",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogLevelId = c.Int(nullable: false),
                        ShortMessage = c.String(nullable: false),
                        FullMessage = c.String(),
                        IpAddress = c.String(maxLength: 200),
                        CustomerId = c.Int(),
                        PageUrl = c.String(),
                        ReferrerUrl = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountUser", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Setting",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Value = c.String(nullable: false, maxLength: 2000),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GenericAttribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        KeyGroup = c.String(nullable: false, maxLength: 400),
                        Key = c.String(nullable: false, maxLength: 400),
                        Value = c.String(nullable: false),
                        StoreId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HashId = c.String(),
                        Subject = c.String(),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AccountUser_AccountUserRole_Mapping",
                c => new
                    {
                        AccountUser_Id = c.Int(nullable: false),
                        AccountUserRole_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AccountUser_Id, t.AccountUserRole_Id })
                .ForeignKey("dbo.AccountUser", t => t.AccountUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.AccountUserRole", t => t.AccountUserRole_Id, cascadeDelete: true)
                .Index(t => t.AccountUser_Id)
                .Index(t => t.AccountUserRole_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Log", "CustomerId", "dbo.AccountUser");
            DropForeignKey("dbo.ActivityLog", "ActivityLogTypeId", "dbo.ActivityLogType");
            DropForeignKey("dbo.ActivityLog", "AccountUserId", "dbo.AccountUser");
            DropForeignKey("dbo.QueuedEmail", "EmailAccountId", "dbo.EmailAccount");
            DropForeignKey("dbo.AccountUser", "Government_Id", "dbo.GovernmentUnit");
            DropForeignKey("dbo.AccountUser_AccountUserRole_Mapping", "AccountUserRole_Id", "dbo.AccountUserRole");
            DropForeignKey("dbo.AccountUser_AccountUserRole_Mapping", "AccountUser_Id", "dbo.AccountUser");
            DropForeignKey("dbo.PropertyRent", "Property_Id", "dbo.Property");
            DropForeignKey("dbo.PropertyRent_Picture_Mapping", "PropertyRentId", "dbo.PropertyRent");
            DropForeignKey("dbo.PropertyRent_Picture_Mapping", "PictureId", "dbo.Pictures");
            DropForeignKey("dbo.PropertyRent_File_Mapping", "PropertyRentId", "dbo.PropertyRent");
            DropForeignKey("dbo.PropertyRent_File_Mapping", "FileId", "dbo.Files");
            DropForeignKey("dbo.Property", "PropertyOff_Id", "dbo.PropertyOff");
            DropForeignKey("dbo.PropertyOff", "Property_Id", "dbo.Property");
            DropForeignKey("dbo.PropertyOff_Picture_Mapping", "PropertyOffId", "dbo.PropertyOff");
            DropForeignKey("dbo.PropertyOff_Picture_Mapping", "PictureId", "dbo.Pictures");
            DropForeignKey("dbo.PropertyOff_File_Mapping", "PropertyOffId", "dbo.PropertyOff");
            DropForeignKey("dbo.PropertyOff_File_Mapping", "FileId", "dbo.Files");
            DropForeignKey("dbo.Property", "PropertyNewCreate_Id", "dbo.PropertyNewCreate");
            DropForeignKey("dbo.PropertyNewCreate", "Property_Id", "dbo.Property");
            DropForeignKey("dbo.Property_Picture_Mapping", "PropertyId", "dbo.Property");
            DropForeignKey("dbo.Property_Picture_Mapping", "PictureId", "dbo.Pictures");
            DropForeignKey("dbo.PropertyLend", "Property_Id", "dbo.Property");
            DropForeignKey("dbo.PropertyLend_Picture_Mapping", "PropertyLendId", "dbo.PropertyLend");
            DropForeignKey("dbo.PropertyLend_Picture_Mapping", "PictureId", "dbo.Pictures");
            DropForeignKey("dbo.PropertyLend_File_Mapping", "PropertyLendId", "dbo.PropertyLend");
            DropForeignKey("dbo.PropertyLend_File_Mapping", "FileId", "dbo.Files");
            DropForeignKey("dbo.Property", "Government_Id", "dbo.GovernmentUnit");
            DropForeignKey("dbo.Property_File_Mapping", "PropertyId", "dbo.Property");
            DropForeignKey("dbo.Property_File_Mapping", "FileId", "dbo.Files");
            DropForeignKey("dbo.PropertyEdit", "Property_Id", "dbo.Property");
            DropForeignKey("dbo.PropertyAllot", "Property_Id", "dbo.Property");
            DropForeignKey("dbo.PropertyAllot_Picture_Mapping", "PropertyAllotId", "dbo.PropertyAllot");
            DropForeignKey("dbo.PropertyAllot_Picture_Mapping", "PictureId", "dbo.Pictures");
            DropForeignKey("dbo.PropertyAllot_File_Mapping", "PropertyAllotId", "dbo.PropertyAllot");
            DropForeignKey("dbo.PropertyAllot_File_Mapping", "FileId", "dbo.Files");
            DropIndex("dbo.AccountUser_AccountUserRole_Mapping", new[] { "AccountUserRole_Id" });
            DropIndex("dbo.AccountUser_AccountUserRole_Mapping", new[] { "AccountUser_Id" });
            DropIndex("dbo.Log", new[] { "CustomerId" });
            DropIndex("dbo.ActivityLog", new[] { "AccountUserId" });
            DropIndex("dbo.ActivityLog", new[] { "ActivityLogTypeId" });
            DropIndex("dbo.QueuedEmail", new[] { "EmailAccountId" });
            DropIndex("dbo.AccountUser", new[] { "Government_Id" });
            DropIndex("dbo.PropertyRent_Picture_Mapping", new[] { "PictureId" });
            DropIndex("dbo.PropertyRent_Picture_Mapping", new[] { "PropertyRentId" });
            DropIndex("dbo.PropertyRent_File_Mapping", new[] { "FileId" });
            DropIndex("dbo.PropertyRent_File_Mapping", new[] { "PropertyRentId" });
            DropIndex("dbo.PropertyRent", new[] { "Property_Id" });
            DropIndex("dbo.PropertyOff_Picture_Mapping", new[] { "PictureId" });
            DropIndex("dbo.PropertyOff_Picture_Mapping", new[] { "PropertyOffId" });
            DropIndex("dbo.PropertyOff_File_Mapping", new[] { "FileId" });
            DropIndex("dbo.PropertyOff_File_Mapping", new[] { "PropertyOffId" });
            DropIndex("dbo.PropertyOff", new[] { "Property_Id" });
            DropIndex("dbo.PropertyNewCreate", new[] { "Property_Id" });
            DropIndex("dbo.Property_Picture_Mapping", new[] { "PictureId" });
            DropIndex("dbo.Property_Picture_Mapping", new[] { "PropertyId" });
            DropIndex("dbo.PropertyLend_Picture_Mapping", new[] { "PictureId" });
            DropIndex("dbo.PropertyLend_Picture_Mapping", new[] { "PropertyLendId" });
            DropIndex("dbo.PropertyLend_File_Mapping", new[] { "FileId" });
            DropIndex("dbo.PropertyLend_File_Mapping", new[] { "PropertyLendId" });
            DropIndex("dbo.PropertyLend", new[] { "Property_Id" });
            DropIndex("dbo.Property_File_Mapping", new[] { "FileId" });
            DropIndex("dbo.Property_File_Mapping", new[] { "PropertyId" });
            DropIndex("dbo.PropertyEdit", new[] { "Property_Id" });
            DropIndex("dbo.PropertyAllot_Picture_Mapping", new[] { "PictureId" });
            DropIndex("dbo.PropertyAllot_Picture_Mapping", new[] { "PropertyAllotId" });
            DropIndex("dbo.PropertyAllot_File_Mapping", new[] { "FileId" });
            DropIndex("dbo.PropertyAllot_File_Mapping", new[] { "PropertyAllotId" });
            DropIndex("dbo.PropertyAllot", new[] { "Property_Id" });
            DropIndex("dbo.Property", new[] { "PropertyOff_Id" });
            DropIndex("dbo.Property", new[] { "PropertyNewCreate_Id" });
            DropIndex("dbo.Property", new[] { "Government_Id" });
            DropTable("dbo.AccountUser_AccountUserRole_Mapping");
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.GenericAttribute");
            DropTable("dbo.Setting");
            DropTable("dbo.Log");
            DropTable("dbo.ActivityLogType");
            DropTable("dbo.ActivityLog");
            DropTable("dbo.QueuedEmail");
            DropTable("dbo.MessageTemplate");
            DropTable("dbo.EmailAccount");
            DropTable("dbo.SubmitRecord");
            DropTable("dbo.CopyProperty");
            DropTable("dbo.MonthTotal");
            DropTable("dbo.AccountUserRole");
            DropTable("dbo.AccountUser");
            DropTable("dbo.PropertyRent_Picture_Mapping");
            DropTable("dbo.PropertyRent_File_Mapping");
            DropTable("dbo.PropertyRent");
            DropTable("dbo.PropertyOff_Picture_Mapping");
            DropTable("dbo.PropertyOff_File_Mapping");
            DropTable("dbo.PropertyOff");
            DropTable("dbo.PropertyNewCreate");
            DropTable("dbo.Property_Picture_Mapping");
            DropTable("dbo.PropertyLend_Picture_Mapping");
            DropTable("dbo.PropertyLend_File_Mapping");
            DropTable("dbo.PropertyLend");
            DropTable("dbo.Property_File_Mapping");
            DropTable("dbo.PropertyEdit");
            DropTable("dbo.Pictures");
            DropTable("dbo.PropertyAllot_Picture_Mapping");
            DropTable("dbo.Files");
            DropTable("dbo.PropertyAllot_File_Mapping");
            DropTable("dbo.PropertyAllot");
            DropTable("dbo.Property");
            DropTable("dbo.GovernmentUnit");
        }
    }
}
