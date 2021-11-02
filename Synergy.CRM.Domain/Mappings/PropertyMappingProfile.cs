namespace Synergy.CRM.Domain.Mappings
{
    using System.Linq;
    using AutoMapper;
    using Synergy.Common.Domain.Models.Common;
    using Synergy.CRM.Models;
    using Synergy.CRM.Models.Property;

    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            this.CreateMap<DAL.Queries.Original.Models.PropertyModel, PropertyModel>()
                .ForMember(x => x.AmountDue, exp => exp.MapFrom(x => x.TotalAmountDue))
                .ForMember(x => x.CadId, exp => exp.MapFrom(x => x.CADId))
                .ForMember(x => x.TaxId, exp => exp.MapFrom(x => x.TAXId))
                .ForMember(x => x.Address, exp => exp.MapFrom(x => new AddressModel { State = new FastEntityModel<int> { Name = x.State }, City = x.City, Zip = x.ZipCode, Address1 = x.Address }))
                .ForMember(x => x.AppraisedValue, exp => exp.MapFrom(x => x.PropertyValuations.OrderByDescending(y => y.AppraisedYear).Select(a => a.AppraisedValue).FirstOrDefault()));

            this.CreateMap<DAL.Queries.Original.Models.PropertyModel, PropertyDetailsModel>()
                .ForMember(x => x.CadId, exp => exp.MapFrom(x => x.CADId))
                .ForMember(x => x.TaxId, exp => exp.MapFrom(x => x.TAXId))
                .ForMember(x => x.SurvivingSpouse, exp => exp.MapFrom(x => x.Over65SurvivingSpouse))
                .ForMember(x => x.TaxDelinquencies, exp => exp.MapFrom(x => x.Delinquencies))
                .ForMember(x => x.Valuations, exp => exp.MapFrom(x => x.PropertyValuations))
                .ForMember(x => x.Address, exp => exp.MapFrom(x => new AddressModel { State = new FastEntityModel<int> { Name = x.State }, City = x.City, Zip = x.ZipCode, Address1 = x.Address }));

            this.CreateMap<DAL.Queries.Original.Models.DelinquencyModel, TaxDelinquencyModel>()
                .ForMember(x => x.AmountDue, exp => exp.MapFrom(x => x.Amount))
                .ForMember(x => x.Year, exp => exp.MapFrom(x => x.DelinquencyYear));

            this.CreateMap<DAL.Queries.Original.Models.PropertyValuationModel, ValuationModel>()
                .ForMember(x => x.Year, exp => exp.MapFrom(x => x.AppraisedYear));

            this.CreateMap<DAL.Queries.Original.Models.CollectingEntityModel, CollectingEntityModel>();

            this.CreateMap<DAL.Queries.Entities.CollectingEntityType, FastEntityModel<int>>()
               .ForMember(x => x.Name, exp => exp.MapFrom(x => x.Description));
        }
    }
}
