using AutoMapper;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.Domain.Mappings
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            this.CreateMap<LeadCommentCreateArgs, LeadCommentCreateCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.CreatedBy, exp => exp.Ignore());
        }
    }
}
