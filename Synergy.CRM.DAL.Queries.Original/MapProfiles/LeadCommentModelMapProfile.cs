using System;
using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class LeadCommentModelMapProfile : Profile
    {
        public LeadCommentModelMapProfile()
        {
            this.CreateMap<LeadComment, LeadCommentModel>()
                .ForMember(e => e.Author, t => t.MapFrom(src => new FastEntityModel<Guid> { Id = src.AuthorId, Name = $"{src.Author.FirstName} {src.Author.LastName}" }))
                ;
        }
    }
}
