using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityDapperCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Portal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserStore<ApplicationUser> _userstore;

        public AccountController(ILogger<HomeController> logger, IUserStore<ApplicationUser> userstore)
        {
            _logger = logger;
            _userstore = userstore;
        }

        public IActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser(ApplicationUser user)
        {
            CancellationTokenSource tokensource = new CancellationTokenSource(60000);


            await _userstore.CreateAsync(user, tokensource.Token);
            return View();
        }
    }
}
