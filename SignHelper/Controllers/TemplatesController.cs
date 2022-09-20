using Microsoft.AspNetCore.Mvc;
using MustIt.Kernel.Controllers;
using SignHelper.Requests.Templates;
using SignHelperApp.Command.Template;
using SignHelperApp.Services.Interfaces;

namespace SignHelper.Controllers
{
    [Controller]
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
            var command = new TemplateAddCommand(request.Name, request.XPosition, request.YPosition);
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
    }
}
