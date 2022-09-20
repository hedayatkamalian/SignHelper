using Microsoft.AspNetCore.Mvc;
using MustIt.Kernel.Controllers;
using SignHelper.Requests.SignRequests;
using SignHelperApp.Command.SignRequest;
using SignHelperApp.Services.Interfaces;

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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Add([FromBody] SignRequestAddRequest request)
        {
            var command = new SignRequestAddCommand(request.FileURL, request.Email, request.TemplateId, request.Description);
            var result = await _signHelperService.SignRequestAdd(command);
            return FromServiceResult(result);
        }
    }
}
