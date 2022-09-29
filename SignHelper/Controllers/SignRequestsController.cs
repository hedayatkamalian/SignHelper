using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MustIt.Kernel.Controllers;
using SignHelper.Requests.SignRequests;
using SignHelperApp.Commands.SignRequests;
using SignHelperApp.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SignHelper.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class SignRequestsController : CustomController
    {
        private readonly ISignHelperService _signHelperService;

        public SignRequestsController(ISignHelperService signHelperService)
        {
            _signHelperService = signHelperService;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Add([FromBody] SignRequestAddRequest request)
        {
            var command = new SignRequestAddCommand(request.FileURL, request.Email, request.TemplateId, request.Description);
            var result = await _signHelperService.SignRequestAdd(command);
            return FromServiceResult(result);
        }

        [HttpGet("{id}/confirm")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Sign([FromRoute] long id, [Required][FromQuery] string code)
        {
            var result = await _signHelperService.SingRequestSign(id, code);
            return FromServiceResult(result);
        }
    }
}
