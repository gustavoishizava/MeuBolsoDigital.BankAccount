using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using MBD.BankAccounts.API.Models;
using MBD.BankAccounts.Application.Interfaces;
using MBD.BankAccounts.Application.Request;
using MBD.BankAccounts.Application.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBD.BankAccounts.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountAppService _service;

        public AccountsController(IAccountAppService service)
        {
            _service = service;
        }

        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<AccountResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var results = await _service.GetAllAsync();
            if (results.IsNullOrEmpty())
                return NoContent();

            return Ok(results);
        }

        [HttpGet("{id:GUID}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.Succeeded)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
        {
            var result = await _service.CreateAsync(request);
            if (!result.Succeeded)
                return BadRequest(new ErrorModel(result));

            return Created($"/api/accounts/{result.Data}", result.Data);
        }

        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UpdateAccountRequest request)
        {
            var result = await _service.UpdateAsync(request);
            if (!result.Succeeded)
                return BadRequest(new ErrorModel(result));

            return NoContent();
        }

        [HttpDelete("{id:GUID}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _service.RemoveAsync(id);
            if (!result.Succeeded)
                return BadRequest(new ErrorModel(result));

            return NoContent();
        }
    }
}