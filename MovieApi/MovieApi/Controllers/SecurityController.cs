using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/security")]
    public class SecurityController: ControllerBase
    {

        private readonly IDataProtector _protector;
        private readonly HashService hashService;

        public SecurityController(IDataProtectionProvider protectionProvider,HashService _hashService)
        {
            _protector = protectionProvider.CreateProtector("A_secret_and_unique_value");
            hashService = _hashService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string plainText = "Vivek Arya";
            string encryptedText = _protector.Protect(plainText);
            string decryptedText = _protector.Unprotect(encryptedText);

            return Ok(new { plainText, encryptedText, decryptedText });
        }

        [HttpGet("TimeBound")]
        //only for the given time you are allowed to decrypt the text
        public async Task<IActionResult> GetTimeBound()
        {
            var protectorTimeBound = _protector.ToTimeLimitedDataProtector();

            string plainText = "Vivek Arya";
            string encryptedText = protectorTimeBound.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));
            await Task.Delay(6000);//we are delaying 6 seconds to check if we'll be allowed to decrypt the text after 6 seconds or not
            string decryptedText = protectorTimeBound.Unprotect(encryptedText);
            return Ok(new { plainText, encryptedText, decryptedText });
        }

        [HttpGet("hash")]
        public IActionResult GetHash()
        {
            var plainText = "Vivek Arya";
            var hashResult1 = hashService.Hash(plainText);
            var hashResult2 = hashService.Hash(plainText);
            return Ok(new { plainText, hashResult1, hashResult2 });
        }

    }
}
