using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Security.Domain.CQRS.Repository.Queries;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.DTO.Response;
using Security.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Security.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        public PermissionsController(IMediator mediator, ILogger<PermissionsController> logger)
        {
            _logger= logger;
            _mediator = mediator;
        }

        [HttpPost("RequestPermission")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RequestPermission([FromBody] RequestPermissionCommand command)
        {
            try
            {
                if (command != null)
                {
                    var result = await _mediator.Send(command);
                    this._logger.LogInformation("RequestPermission success - Employee #" + result.Id);
                    return Ok(result);
                }
                else
                {
                    this._logger.LogInformation("ModifyPermission error - invalid request");
                    return BadRequest();
                }
            }
            catch (Exception exp)
            {
                this._logger.LogError("RequestPermission error: " + exp.Message);
                return BadRequest(exp.Message);
            }
        }

        [HttpGet("GetPermissions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<Permissions>> GetPermissions()
        {
            this._logger.LogInformation("GetPermissions success");
            return await _mediator.Send(new GetPermissionsQuery());
        }

        [HttpPost("ModifyPermission/{id}")]
        public async Task<ActionResult> ModifyPermission(int id, [FromBody] ModifyPermissionCommand command)
        {
            try
            {
                if (command.Id == id)
                {
                    var result = await _mediator.Send(command);
                    this._logger.LogInformation("ModifyPermission success - Employee #" + result.Id);
                    return Ok(result);
                }
                else
                {
                    this._logger.LogInformation("ModifyPermission error - invalid request");
                    return BadRequest();
                }
            }
            catch (Exception exp)
            {
                this._logger.LogError("ModifyPermission error: " + exp.Message);
                return BadRequest(exp.Message);
            }
        }

        [HttpGet("Test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Test()
        {
            return Ok("Llamado");
        }
    }
}
