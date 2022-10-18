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
            var command = new SignRequestAddCommand
                (request.FileURL,
                request.RecipientEmail,
                request.SingerEmail,
                request.SignerPhoneNumber,
                request.TemplateId
                , request.Description);

            var result = await _signHelperService.SignRequestAdd(command);
            return FromServiceResult(result);
        }

        [HttpPost("{id}/sign")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Sign([FromRoute] long id, [Required][FromBody] SignRequestSign request)
        {
            var command = new SignRequestSignCommand(id, request.Code, request.SignImage);
            var result = await _signHelperService.SingRequestSign(command);
            return FromServiceResult(result);
        }

        [HttpPost("{id}/confirm")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Sign([FromRoute] long id, [Required][FromBody] SignRequestConfirm request)
        {
            var result = await _signHelperService.SingRequestConfirm(id, request.Code);
            return FromServiceResult(result);
        }

        [HttpPost("{id}/send-confirm-code")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendConfirmCode([FromRoute] long id)
        {
            var result = await _signHelperService.SignRequestSendConfirmCode(id);
            return FromServiceResult(result);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> Get([FromRoute] long id)
        {
            var result = await _signHelperService.SignRequestGet(id);
            return FromServiceResult(result);
        }
    }
}
