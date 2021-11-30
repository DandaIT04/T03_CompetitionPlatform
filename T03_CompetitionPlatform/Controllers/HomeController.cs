using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using T03_CompetitionPlatform.Models;
using Microsoft.AspNetCore.Http;
using T03_CompetitionPlatform.DAL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication.Cookies;
using Google.Apis.Auth.OAuth2;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Google.Apis.Auth;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace T03_CompetitionPlatform.Controllers
{
    

    public class HomeController : Controller
    {
        private JudgesDAL judgeContext = new JudgesDAL();
        
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private CompetitionDAL competitionContext = new CompetitionDAL();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        // Activates redirect to login button, to login.cshtml
        [HttpPost]
        public ActionResult redirectLogin(IFormCollection formData)
        {
            return View("Login");
        }

        // Activates redirect to register button, to register.cshtml
        [HttpPost]
        public ActionResult redirectRegister(IFormCollection formData)
        {
            return RedirectToAction("Register");
        }

        [HttpPost]
        public ActionResult cjpLogin(IFormCollection formData)
        {
            // Read inputs from textboxes
            // Email address converted to lowercase
            
            string loginID = formData["txtLoginID"].ToString();
            string password = formData["txtPassword"].ToString();
            List<Judge> judgeList = judgeContext.GetAllJudges();
            List<Competitor> competitorList = competitorContext.GetAllCompetitors();
            if (loginID == "admin1@lcu.edu.sg" && password == "p@55Admin")
            {
                //Store login ID in session with the key "LoginID"
                HttpContext.Session.SetString("LoginID", loginID);

                //Store user role "Admin" as a string in session with the key "Role"
                HttpContext.Session.SetString("Role", "Admin");

                //Get a list of competitions using CompetitionDAL
                List<Competition> competitionList = competitionContext.GetAllCompetitions();
                foreach (Competition c in competitionList)
                {
                    //Assign a value of 0 as an int in session with the key of each competition ID, to indicate that this user has not voted for any comps.
                    string compID = c.CompetitionID.ToString();
                    HttpContext.Session.SetInt32(compID, 0);
                }


                // Redirect user to the "AdminMain" view through an action
                return RedirectToAction("AdminMain", "Admin");
            }
            else
            {

                foreach (var item in judgeList)
                {
                    if (loginID == item.EmailAddr && password == item.Password)
                    {
                        Debug.WriteLine("My debug string here");

                        //Store login ID in session with the key "LoginID"
                        HttpContext.Session.SetString("LoginID", loginID);

                        //Store user role "Judge" as a string in session with the key "Role"
                        HttpContext.Session.SetString("Role", "Judge");

                        TempData["Loggedin"] = item.EmailAddr;

                        //Get a list of competitions using CompetitionDAL
                        List<Competition> competitionList = competitionContext.GetAllCompetitions();
                        foreach (Competition c in competitionList)
                        {
                            //Assign a value of 0 as an int in session with the key of each competition ID, to indicate that this user has not voted for any comps.
                            string compID = c.CompetitionID.ToString();
                            HttpContext.Session.SetInt32(compID, 0);
                        }



                        // Redirect user to the "Index" view through an action
                        return RedirectToAction("Index", "Judge");
                    };
                };
                foreach (var item2 in competitorList)
                {
                    if (loginID == item2.EmailAddr && password == item2.Password)
                    {
                        //Store login ID in session with the key "LoginID"
                        HttpContext.Session.SetString("LoginID", loginID);

                        //Store login ID in session with the key "CompetitorID"
                        HttpContext.Session.SetInt32("CompetitorID", item2.CompetitorID);

                        //Store user role "Competitor" as a string in session with the key "Role"
                        HttpContext.Session.SetString("Role", "Competitor");

                        //Get a list of competitions using CompetitionDAL
                        List<Competition> competitionList = competitionContext.GetAllCompetitions();
                        foreach (Competition c in competitionList)
                        {
                            //Assign a value of 0 as an int in session with the key of each competition ID, to indicate that this user has not voted for any comps.
                            string compID = c.CompetitionID.ToString();
                            HttpContext.Session.SetInt32(compID, 0);
                        }
                        // Redirect user to the "Index" view through an action
                        return RedirectToAction("Index", "Competitor");
                    }

                };
            // Store an error message in TempData for display at the index view
            TempData["Message"] = "Invalid Login Credentials!";

            // Redirect user back to the index view through an action
            return RedirectToAction("Index");
            }
        }

        [Authorize]
        public async Task<ActionResult> GoogleLogin()
        {
            // The user is already authenticated, so this call won't
            // trigger login, but it allows us to access token related values.
            AuthenticateResult auth = await HttpContext.AuthenticateAsync();
            string idToken = auth.Properties.GetTokenValue(
             OpenIdConnectParameterNames.IdToken);
            try
            {
                // Verify the current user logging in with Google server
                // if the ID is invalid, an exception is thrown
                Payload currentUser = await
                GoogleJsonWebSignature.ValidateAsync(idToken);
                string userName = currentUser.Name;
                string eMail = currentUser.Email;

                List<Judge> judgeList = judgeContext.GetAllJudges();
                List<Competitor> competitorList = competitorContext.GetAllCompetitors();

                bool isCompetitor = false;
                bool isJudge = false;

                foreach (var item2 in competitorList)
                {
                    if (item2.EmailAddr == eMail)
                    {
                        //Store login ID in session with the key "LoginID"
                        HttpContext.Session.SetString("LoginID", eMail);

                        //Store login ID in session with the key "CompetitorID"
                        HttpContext.Session.SetInt32("CompetitorID", item2.CompetitorID);

                        //Store user role "Competitor" as a string in session with the key "Role"
                        HttpContext.Session.SetString("Role", "Competitor");

                        HttpContext.Session.SetString("LoggedInTime",
                        DateTime.Now.ToString());

                        isCompetitor = true;
                    }
                };
                if (isCompetitor != true)
                {
                    foreach (var item2 in judgeList)
                    {
                        if (item2.EmailAddr == eMail)
                        {
                            //Store login ID in session with the key "LoginID"
                            HttpContext.Session.SetString("LoginID", eMail);

                            //Store login ID in session with the key "CompetitorID"
                            HttpContext.Session.SetInt32("JudgeID", item2.JudgeID);

                            //Store user role "Judge" as a string in session with the key "Role"
                            HttpContext.Session.SetString("Role", "Judge");

                            HttpContext.Session.SetString("LoggedInTime",
                            DateTime.Now.ToString());

                            isJudge = true;
                            TempData["Loggedin"] = item2.EmailAddr;
                        }

                    };
                }
                
                string theRedirect = "";

                if (isCompetitor == true)
                {
                    theRedirect = "Competitor";

                }

                if (isJudge == true)
                {
                    theRedirect = "Judge";

                }

                //Get a list of competitions using CompetitionDAL
                List<Competition> competitionList = competitionContext.GetAllCompetitions();
                foreach (Competition c in competitionList)
                {
                    //Assign a value of 0 as an int in session with the key of each competition ID, to indicate that this user has not voted for any comps.
                    string compID = c.CompetitionID.ToString();
                    HttpContext.Session.SetInt32(compID, 0);
                }

                if (theRedirect == "")
                {
                    // Clear authentication cookie
                    await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpContext.Session.Clear();

                    theRedirect = "Home";
                }

                // Redirect user to the "Index" view through an action
                return RedirectToAction("Index", theRedirect);
            }

            catch (Exception e)
            {
                // Clear authentication cookie
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

                HttpContext.Session.Clear();
                return RedirectToAction("Index");
                // Token ID is may be tempered with, force user to logout
            }
        }

        [HttpPost]
        public ActionResult GuestLogin()
        {
            //Get a list of competitions using CompetitionDAL
            List<Competition> competitionList = competitionContext.GetAllCompetitions();
            foreach (Competition c in competitionList)
            {
                //Assign a value of 0 as an int in session with the key of each competition ID, to indicate that this user has not voted for any comps.
                string compID = c.CompetitionID.ToString();
                HttpContext.Session.SetInt32(compID, 0);
            }


            //Store user role "Guest" in session with the key "Role"
            HttpContext.Session.SetString("Role", "Guest");

            // Redirect user to the "Index" view through an action
            return RedirectToAction("Index", "Guest");
        }


        public ActionResult guestLogout()
        {
            // Clear all session values, such as 'Role' and competition vote values
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult adminLogout()
        {
            HttpContext.Session.Clear();

            //Call the index action of Home Controller 
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> competitorLogout()
        {
            // Clear authentication cookie
            await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> judgeLogout()
        {
            // Clear authentication cookie
            await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }




        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
