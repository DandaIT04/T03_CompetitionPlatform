using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T03_CompetitionPlatform.DAL;
using T03_CompetitionPlatform.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;


namespace T03_CompetitionPlatform.Controllers
{
    public class CompetitorController : Controller
    {
        private CompetitionDAL competitionContext = new CompetitionDAL();
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private CompetitionSubmissionDAL competitionSubmissionContext = new CompetitionSubmissionDAL();

        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Competitor"))
            {
                return RedirectToAction("Index", "Home");
            }

            List<Competition> competitionList = competitionContext.GetAllCompetitions();

            // To make sure competitor can only see competitions that he can join,
            // Which is 3 days before the start date
            // and also not after the end date.
            // Unable to test because I cannot make a Competition to start before today date.
            foreach (Competition compt in competitionList.ToList())
            {
                if (DateTime.Now.AddDays(3) >= compt.StartDate)
                {
                    competitionList.Remove(compt);
                }
                else if (DateTime.Now > compt.EndDate)
                {
                    competitionList.Remove(compt);
                }
            }
            return View(competitionList);
        }

        // GET: Competitor/Register
        [HttpGet]
        public ActionResult CompetitorRegister()
        {
            // In case need again.
            ViewData["SalutationList"] = GetSalutations();
            return View();
        }

        // List for salutations
        private List<SelectListItem> GetSalutations()
        {
            List<SelectListItem> salutations = new List<SelectListItem>();
            salutations.Add(new SelectListItem { Value = "Dr", Text = "Dr" });
            salutations.Add(new SelectListItem { Value = "Mr", Text = "Mr" });
            salutations.Add(new SelectListItem { Value = "Ms", Text = "Ms" });
            salutations.Add(new SelectListItem { Value = "Mrs", Text = "Mrs" });
            salutations.Add(new SelectListItem { Value = "Mdm", Text = "Mdm" });

            return salutations;
        }

        // POST: Competitor/Register
        [HttpPost]
        public ActionResult CompetitorRegister(Competitor compt)
        {
            // In case need again.
            ViewData["SalutationList"] = GetSalutations();

            if (ModelState.IsValid)
            {
                compt.CompetitorID = competitorContext.CompetitorRegister(compt);

                return RedirectToAction("Index");
            }
            else
            {
                return View(compt);
            }
        }

        // POST: Competitor/JoinCompetition
        [HttpGet]
        public ActionResult JoinCompetition(CompetitionSubmission comptSub)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Competitor"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["SalutationList"] = GetSalutations();
            comptSub.CompetitorID = (int)HttpContext.Session.GetInt32("CompetitorID");
            // VoteCount starts at 0
            comptSub.VoteCount = 0;

            if (ModelState.IsValid)
            {
                // Successful addition to database, however there is no message to show that it succeeded
                // or that the user cannot see if he has already joined the competition nor if he can
                // join due to the date.

                // Add to database
                competitionSubmissionContext.JoinCompetition(comptSub);

                // Redirect to list view
                return RedirectToAction("Index");
            }
            else
            {
                // Redirect to list view
                return RedirectToAction("Index");
            }
        }

        // GET: CompetitorController JoinedCompetitions
        public ActionResult JoinedCompetitions()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Competitor"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<CompetitionSubmission> comptSubList = competitionSubmissionContext.GetDetails((int)HttpContext.Session.GetInt32("CompetitorID"));
            return View("ViewJoinedCompetitions", comptSubList);
        }

        public ActionResult UploadPhoto(int id) //CompetitionID
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Competitor"))
            {
                return RedirectToAction("Index", "Home");
            }
            int CompetitorID = (int)HttpContext.Session.GetInt32("CompetitorID");
            CompetitionSubmission comptSub = competitionSubmissionContext.GetSingleDetails(id, CompetitorID);
            return View("UploadPhoto", comptSub);
        }
    }
}
