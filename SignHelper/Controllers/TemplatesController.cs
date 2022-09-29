using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MustIt.Kernel.Controllers;
using SignHelper.Requests.Templates;
using SignHelperApp.Commands.Templates;
using SignHelperApp.Entities;
using SignHelperApp.Services.Interfaces;

namespace SignHelper.Controllers
{
    [Controller]
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    public class TemplatesController : CustomController
    {
        private readonly ISignHelperService _signHelperService;

        public TemplatesController(ISignHelperService signHelperService)
        {
            _signHelperService = signHelperService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Add([FromBody] TemplateAddRequest request)
        {

            var signPoints = new List<SignPoint>();

            foreach (var p in request.SingPoints)
            {
                signPoints.Add(new SignPoint
                {
                    Page = p.Page,
                    X = p.X,
                    Y = p.Y
                });
            }

            var command = new TemplateAddCommand(request.Name, signPoints, request.ImageName, request.Width, request.Height);
            var result = await _signHelperService.TemplateAdd(command);
            return FromServiceResult(result);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            var result = await _signHelperService.TemplateDelete(id);
            return FromServiceResult(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _signHelperService.TemplateGetAll();
            return FromServiceResult(result);
        }
    }
}
