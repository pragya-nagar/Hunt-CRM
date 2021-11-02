using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class CreateLeadCommentMapProfile : Profile
    {
        public CreateLeadCommentMapProfile()
        {
            this.CreateMap<CreateLeadCommentModel, LeadComment>()
                .IgnoreAuditMembers()
                .ForMember(c => c.Author, src => src.Ignore())
                .ForMember(c => c.CommentDate, src => src.Ignore())
                .ForMember(c => c.Lead, src => src.Ignore())
                .ForMember(c => c.AuthorId, src => src.Ignore())
                ;
        }
    }
}
