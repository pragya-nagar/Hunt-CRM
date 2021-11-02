namespace Synergy.CRM.API.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Synergy.Common.Domain.Models.Common;
    using Synergy.Common.Security.Attributes;
    using Synergy.CRM.Domain.Abstracts;
    using Synergy.CRM.Models;
    using Synergy.DataAccess.Enum;

    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public class PropertiesController : Controller
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            this._propertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
        }

        [CheckPermission("CRM.Properties.Read")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<PropertyModel>), 200)]
        public async Task<IActionResult> Get([FromQuery]SearchArgsModel<PropertyFilterArgs, PropertySortField> args, CancellationToken cancellationToken = default)
        {
            var result = await this._propertyService.GetListAsync(args, cancellationToken).ConfigureAwait(false);
            return this.Ok(result);
        }

        [CheckPermission("CRM.Properties.Read")]
        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(PropertyDetailsModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute]Guid id, CancellationToken cancellationToken = default)
        {
            var item = await this._propertyService.FindAsync(id, cancellationToken).ConfigureAwait(false);
            return this.Ok(item);
        }
    }
}
