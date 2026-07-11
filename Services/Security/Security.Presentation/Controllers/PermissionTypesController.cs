using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Security.Domain.CQRS.Repository.Queries;
using Security.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Security.Presentation.Controllers
{
    /// <summary>
    /// Permission types lookup API controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public PermissionTypesController(IMediator mediator, ILogger<PermissionTypesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all permission types (used to populate the type dropdown when requesting/modifying a permission)
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PermissionsType>>> Get()
        {
            try
            {
                var result = await _mediator.Send(new GetPermissionTypesQuery());
                return Ok(result);
            }
            catch (Exception exp)
            {
                this._logger.LogError("GetPermissionTypes error: " + exp.Message);
                return BadRequest(exp.Message);
            }
        }
    }
}
