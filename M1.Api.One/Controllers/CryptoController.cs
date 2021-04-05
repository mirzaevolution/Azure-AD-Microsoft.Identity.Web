using Microsoft.AspNetCore.Mvc;
using System;
using M1.Api.Core;
using M1.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace M1.Api.One.Controllers
{
    [Route("api/[controller]")]
    [ApiController,Authorize]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoOperation _cryptoOperation;
        public CryptoController(ICryptoOperation cryptoOperation)
        {
            _cryptoOperation = cryptoOperation;
        }
        [HttpPost("tdes/encrypt")]
        public IActionResult Encrypt([FromBody]EncryptRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string result = _cryptoOperation.Encrypt(request.PlainText);
                    return Ok(new EncryptResponse
                    {
                        CipherText = result
                    });
                }
                catch(Exception ex)
                {
                    return StatusCode(500, new
                    {
                        errorMessage = ex.Message
                    });
                }
            }
            else
            {
                return BadRequest(new
                {
                    errorMessage = "Invalid payload"
                });
            }
        }
        [HttpPost("tdes/decrypt")]
        public IActionResult Decrypt([FromBody]DecryptRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string result = _cryptoOperation.Decrypt(request.CipherText);
                    return Ok(new DecryptResponse
                    {
                        PlainText = result
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        errorMessage = ex.Message
                    });
                }
            }
            else
            {
                return BadRequest(new
                {
                    errorMessage = "Invalid payload"
                });
            }
        }
    }
}
