using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Synergy.Common.DAL.Access.PostgreSQL;
using Synergy.CRM.DAL.Queries.Entities;

namespace Synergy.CRM.DAL.Queries.PostgreSQL
{
    public class DataAccess : BaseDataAccess
    {
        private const string Schema = "main";

        public DataAccess(ILoggerFactory loggerFactory, string nameOrConnectionString)
            : base(loggerFactory, nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema(Schema);

            builder.Entity<Campaign>().HasKey(x => x.Id);
            builder.Entity<Campaign>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<Campaign>().Property(x => x.Name).HasColumnName("CampaignName");
            builder.Entity<Campaign>().Property(x => x.TypeId).HasColumnName("CampaignTypeId");
            builder.Entity<Campaign>().Property(x => x.SubTypeId).HasColumnName("CampaignSubTypeId");
            builder.Entity<Campaign>().HasOne(x => x.AssignedUser).WithMany().HasForeignKey(x => x.AssignedUserId);
            builder.Entity<Campaign>().HasOne(x => x.Type).WithMany().HasForeignKey(x => x.TypeId);
            builder.Entity<Campaign>().HasOne(x => x.SubType).WithMany().HasForeignKey(x => x.SubTypeId);
            builder.Entity<Campaign>().HasMany(x => x.LeadLinks).WithOne(x => x.Campaign).HasForeignKey(x => x.CampaignId);

            builder.Entity<CampaignComment>().HasOne(x => x.Campaign).WithMany().HasForeignKey(x => x.CampaignId);

            builder.Entity<Property>().HasKey(x => x.Id);
            builder.Entity<Property>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<Property>().Property(x => x.ParcelId);
            builder.Entity<Property>().HasOne(x => x.State).WithMany().HasForeignKey(x => x.StateId);
            builder.Entity<Property>().HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadId);
            builder.Entity<Property>().HasOne(x => x.InternalLandUseCode).WithMany().HasForeignKey(x => x.InternalLandUseCodeId);
            builder.Entity<Property>().HasOne(x => x.GeneralLandUseCode).WithMany().HasForeignKey(x => x.GeneralLandUseCodeId);
            builder.Entity<Property>().HasMany(x => x.Valuations).WithOne(x => x.Property).HasForeignKey(x => x.PropertyId);

            builder.Entity<Lead>().HasKey(x => x.Id);
            builder.Entity<Lead>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<Lead>().HasMany(x => x.CampaignLinks).WithOne(x => x.Lead).HasForeignKey(x => x.LeadId);
            builder.Entity<Lead>().HasMany(x => x.Properties).WithOne(x => x.Lead).HasForeignKey(x => x.LeadId);
            builder.Entity<Lead>().HasMany(x => x.Contacts).WithOne(x => x.Lead).HasForeignKey(x => x.LeadId);

            builder.Entity<LeadComment>().HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadId);

            builder.Entity<Contact>().HasKey(x => x.Id);
            builder.Entity<Contact>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<Contact>().HasOne(x => x.ContactType).WithMany().HasForeignKey(x => x.ContactTypeId);

            builder.Entity<User>().HasKey(x => x.Id);
            builder.Entity<User>().Property(x => x.Id).ValueGeneratedNever();

            builder.Entity<State>().HasKey(x => x.Id);
            builder.Entity<State>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<State>().Property(x => x.Name);
            builder.Entity<State>().Property(x => x.Abbreviation);

            builder.Entity<GeneralLandUseCode>().HasKey(x => x.Id);
            builder.Entity<GeneralLandUseCode>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<GeneralLandUseCode>().Property(x => x.Name);

            builder.Entity<InternalLandUseCode>().HasKey(x => x.Id);
            builder.Entity<InternalLandUseCode>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<InternalLandUseCode>().Property(x => x.Name);
            builder.Entity<InternalLandUseCode>().Property(x => x.Description);

            builder.Entity<CampaignRule>().HasKey(x => x.Id);
            builder.Entity<CampaignRule>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<CampaignRule>().Property(x => x.Name);
            builder.Entity<CampaignRule>().HasMany(x => x.Items);

            builder.Entity<CampaignRuleCampaign>();
            builder.Entity<CampaignRuleCampaign>().HasKey(x => x.Id);
            builder.Entity<CampaignRuleCampaign>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<CampaignRuleCampaign>().HasOne(x => x.CampaignRule).WithMany(x => x.CampaignLinks).HasForeignKey(x => x.CampaignRuleId);

            builder.Entity<CampaignRuleItem>().HasKey(x => x.Id);
            builder.Entity<CampaignRuleItem>().Property(x => x.Id).ValueGeneratedNever();

            builder.Entity<Opportunity>().HasKey(x => x.Id);
            builder.Entity<Opportunity>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<Opportunity>().HasMany(x => x.OpportunityProperties).WithOne(x => x.Opportunity).HasForeignKey(x => x.OpportunityId);
            builder.Entity<Opportunity>().HasMany(x => x.OpportunityBorrowers).WithOne(x => x.Opportunity).HasForeignKey(x => x.OpportunityId);
            builder.Entity<Opportunity>().HasMany(x => x.OpportunityCommercialBorrowers).WithOne(x => x.Opportunity).HasForeignKey(x => x.OpportunityId);

            builder.Entity<OpportunityAudit>().HasKey(x => new { x.Id, x.InsertedOn });

            builder.Entity<OpportunityPropertyType>().HasKey(x => x.Id);
            builder.Entity<OpportunityPropertyType>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<OpportunityPropertyType>().Property(x => x.Name);
            builder.Entity<OpportunityPropertyType>().Property(x => x.Description);

            builder.Entity<OpportunityMonthlyPrepay>().HasKey(x => x.Id);
            builder.Entity<OpportunityMonthlyPrepay>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<OpportunityMonthlyPrepay>().Property(x => x.MonthlyPrepay);

            builder.Entity<OpportunityPercentagePrepay>().HasKey(x => x.Id);
            builder.Entity<OpportunityPercentagePrepay>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<OpportunityPercentagePrepay>().Property(x => x.PercentagePrepay);

            builder.Entity<OpportunityBorrowerBase>().HasKey(x => x.Id);
            builder.Entity<OpportunityBorrowerBase>().Property(x => x.Id).ValueGeneratedNever();

            builder.Entity<OpportunityBorrowerBaseAudit>().HasKey(x => new { x.Id, x.InsertedOn });
            builder.Entity<OpportunityPropertyAudit>().HasKey(x => new { x.Id, x.InsertedOn });

            builder.Entity<ContactAudit>().HasKey(x => new { x.Id, x.InsertedOn });
            builder.Entity<PropertyAudit>().HasKey(x => new { x.Id, x.InsertedOn });

            builder.Entity<CollectingEntityType>().HasKey(x => x.Id);

            base.OnModelCreating(builder);
        }
    }
}
