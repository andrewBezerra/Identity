using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityDapperCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portal.Models;

namespace Portal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly RoleManager<ApplicationRole> _rolemanager;

        public AccountController(ILogger<HomeController> logger, 
                                UserManager<ApplicationUser> usermanager,
                                RoleManager<ApplicationRole> rolemanager)
        {
            _logger = logger;
          
            _usermanager = usermanager;
            _rolemanager = rolemanager;
        }

        public IActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                CancellationTokenSource tokensource = new CancellationTokenSource(60000);

                ApplicationUser NewUser = new ApplicationUser()
                {
                    Email=user.Email,
                    UserName=user.Email
                };

                


                var result = await _usermanager.CreateAsync(NewUser, user.Senha);

                if (result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(error.Code, error.Description);
                    return View(user);
                }
                var usercreated = await _usermanager.FindByEmailAsync(user.Email);

                var resultrole =await _usermanager.AddToRoleAsync(usercreated, "Admin");

            }
            return RedirectToAction("Index", "Home");
            
        }
    }
}
