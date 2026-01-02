using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.Models;
using ResumeAnalyzer.Services;
using ResumeAnalyzer.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ResumeAnalyzer.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly IResumeAnalyzerService _analyzerService;
        private readonly IEmailService _emailService;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, IResumeAnalyzerService analyzerService, IEmailService emailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _analyzerService = analyzerService;
            _emailService = emailService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
               
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "ResumeAnalyze");
                }
            }
            else
            {
                ModelState.AddModelError("", "Email or password Incorrect");
                return View(model);
            }
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                Users user = new Users()
                {
                    FullName = model.Name,
                    Email = model.Email,
                    UserName = model.Email
                };


                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                return View(model);


            }
            return View(model);
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit code
        }
        public IActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Something went wrong");
                    // Logic to send verification email goes here
                    return View(model);
                }
                else
                {
                    var otp = GenerateOtp();
                    TempData["OTP"] = otp;
                    TempData["OTP_Email"] = user.Email;
                    TempData["OTP_Expires"] = DateTime.UtcNow.AddMinutes(10);


                    //use this if you want to send the Token

                    //var token = await userManager.GenerateUserTokenAsync(
                    //        user,
                    //        TokenOptions.DefaultProvider,
                    //                         "PasswordResetOTP"
                    //         );


                    await _emailService.SendAsync(
                            model.Email,
                            "Verification Code",
                            $"Your verification code is: {otp} It will expire in 10 minutes."
   );
                    return RedirectToAction("VerifyCode", "Account", new { username = user.UserName });
                    // return RedirectToAction("ChangePassword", "Account",new {username = user.UserName});
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult VerifyCode(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new VerifyCodeVM { Email = username });
        }
        [HttpPost]
        public async Task<IActionResult> VerifyTokenCode(VerifyCodeVM model)
        {
            
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("VerifyEmail");

            var isValid = await userManager.VerifyUserTokenAsync(
                user,
                TokenOptions.DefaultProvider,
                "PasswordResetOTP",
                model.Code
            );

            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid or expired code");
                var newModel = new VerifyCodeVM
                {
                    Email = model.Email,
                    Code = string.Empty // Force code to be empty
                };

                // Clear ModelState completely to ensure Razor doesn't reuse old values
                ModelState.Clear();
                return View(newModel);
            }

            TempData["VerifiedEmail"] = model.Email;
            return RedirectToAction("ChangePassword", "Account", new {username = model.Email});
        }
        [HttpPost]
        public IActionResult VerifyCode(VerifyCodeVM model)
        {
            var savedOtp = TempData["OTP"]?.ToString();
            var savedEmail = TempData["OTP_Email"]?.ToString();
            var expires = TempData["OTP_Expires"] as DateTime?;

            if (savedEmail != model.Email)
            {
                ModelState.AddModelError("", "Email mismatch");
                var newModel = new VerifyCodeVM
                {
                    Email = model.Email,
                    Code = string.Empty // Force code to be empty
                };

                // Clear ModelState completely to ensure Razor doesn't reuse old values
                ModelState.Clear();
                return View(newModel);
              
            }

            if (expires == null || DateTime.UtcNow > expires.Value)
            {
                ModelState.AddModelError("", "OTP expired. Please request a new code.");
                var newModel = new VerifyCodeVM
                {
                    Email = model.Email,
                    Code = string.Empty // Force code to be empty
                };

                // Clear ModelState completely to ensure Razor doesn't reuse old values
                ModelState.Clear();
                return View(newModel);
            }

            if (savedOtp != model.Code)
            {
                ModelState.AddModelError("", "Invalid code. Try again.");

                // Keep OTP for another attempt
                TempData.Keep("OTP");
                TempData.Keep("OTP_Email");
                TempData.Keep("OTP_Expires");

                var newModel = new VerifyCodeVM
                {
                    Email = model.Email,
                    Code = string.Empty // Force code to be empty
                };

                // Clear ModelState completely to ensure Razor doesn't reuse old values
                ModelState.Clear();
                return View(newModel);
            }

            // ✅ OTP verified
            TempData["VerifiedEmail"] = model.Email;

            return RedirectToAction("ChangePassword", "Account", new { username = model.Email });
        }


        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordVM { Email = username });
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email not Found!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Email not Found!");
                return View(model);
            }

        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


    }
}
