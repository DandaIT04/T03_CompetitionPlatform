using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using T03_CompetitionPlatform.DAL;
using T03_CompetitionPlatform.Models;

namespace T03_CompetitionPlatform.Controllers
{
    public class GuestController : Controller
    {
        // Get all the DAL objects prepared 
        private CompetitionDAL competitionContext = new CompetitionDAL();
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private CompetitionSubmissionDAL competitionSubmissionContext = new CompetitionSubmissionDAL();
        private CommentDAL commentContext = new CommentDAL();
        private CompetitionScoreDAL compscoreContext = new CompetitionScoreDAL();

        public ActionResult Index()
        {
            // Put the data from the CompetitionDAL function called from the object into a list containing all the competitions
            List<Competition> compList = competitionContext.GetAllCompetitions();
            return View(compList);   
        }

        // Triggered when a user clicks to view a competition, where they will 'view competitors' in that specific competition
        public ActionResult ViewCompetitors(int? id, string name)
        {
            // Put the data from the CompetitionSubmissionDAL function called from the object into a list, containing all the comp submissions
            List<CompetitionSubmission> compSubmission = competitionSubmissionContext.GetAllSubmissions();
            List<CompetitionSubmission> specifiedSubmission = new List<CompetitionSubmission>();

            // Iterate through each comp submission in database to put the comp submissions in specific competition into a list, 'specifiedSubmission'
            foreach(CompetitionSubmission cs in compSubmission){
                if (cs.CompetitionID == id)
                {
                    specifiedSubmission.Add(cs);
                }
            }

            // Put the data from the CompetitorDAL function called from the object into a list containing all the competitors
            List<Competitor> competitorList = competitorContext.GetAllCompetitors();
            List<CompSubmissionViewModel> currentSubmissions = new List<CompSubmissionViewModel>();

            // Iterate through each competitor to see if they have submitted any works
            foreach(Competitor c in competitorList)
            {
                foreach (CompetitionSubmission cs in specifiedSubmission)
                {
                    if (c.CompetitorID == cs.CompetitorID)
                    {
                        // Create a ViewModel object that better suites the view
                        CompSubmissionViewModel csVM = MapTocsVM(cs, id);

                        // Add ViewModel object into an IEnumerable list
                        currentSubmissions.Add(csVM);
                    }
                } 
            }

            // Set ViewData values to display on the view
            ViewData["CompName"] = name;
            ViewData["CompID"] = id;

            // Put the data from the CompetitionDAL function called from the object into a list containing all the competitions
            List<Competition> competitions = competitionContext.GetAllCompetitions();

            // Iterate through each competition to set a ViewData value for 'Status', in terms of whether they're ongoing, ended or not ongoing.
            foreach (Competition x in competitions)
            {
                if (x.CompetitionID == id)
                {
                    if (DateTime.Now < x.StartDate)
                    {
                        ViewData["Status"] = "Not ongoing";
                    }else if (DateTime.Now > x.EndDate)
                    {
                        ViewData["Status"] = "Ended";
                    }else if (DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate)
                    {
                        ViewData["Status"] = "Ongoing";
                    }
                }
            }

            
            // Return view with a list of CompSubmissionViewModel objects
            return View(currentSubmissions);
        }

        // Function is called when a public member clicks to view the comments section
        public ActionResult GuestComment(int? id)
        {
            // Set ViewData of CompID to display in the upcoming view
            ViewData["CompID"] = id;

            // Put the data from the CompetitionDAL function called from the object into a list containing all the competitions
            List<Competition> comps = competitionContext.GetAllCompetitions();

            //Iterate through  every competition to set a ViewData value for CompName
            foreach (Competition c in comps)
            {
                if (c.CompetitionID == id)
                {
                    ViewData["CompName"] = c.CompetitionName;
                    break;
                }
            }
            // Put the data from the CommentDAL function called from the object into a list containing all the comments
            List<Comment> comments = commentContext.GetAllComments();
            List<Comment> relevantComments = new List<Comment>();

            //Iterate through each comments to add comments under the specific competition into the list 'relevantComments'
            foreach (Comment c in comments)
            {
                if(c.CompetitionID == id)
                {
                    relevantComments.Add(c);
                }
            }

            // Returns a view with a list of comments relevant to the competition specified
            return View(relevantComments);
        }

        // Function is called when a public member clicks the button to initiate posting comment
        public ActionResult PostComment(int? id)
        {
            // Assign ViewData value of CompID for the subsequent view
            ViewData["CompID"] = id;
            return View();
        }

        // Function is called when a specific submission is viewed
        public ActionResult ViewCompetitorWork(int? competitorID, int? competitionID)
        {
            // Put the data from the CompetitionSubmissionDAL function called from the object into a list containing all the comp submmissions
            List<CompetitionSubmission> compSubmission = competitionSubmissionContext.GetAllSubmissions();

            // Iterate through each comp submission to find the (ONE) specific comp submission
            foreach (CompetitionSubmission cs in compSubmission)
            {
                if (cs.CompetitorID == competitorID && cs.CompetitionID== competitionID)
                {
                    // Convert that comp submission into a ViewModel object, using MapTocsVM() and return to the subsequent view
                    return View(MapTocsVM(cs, cs.CompetitionID));
                }
            }
            return View();
        }

        
        // Function is called when a vote is to be added
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addVote(CompSubmissionViewModel csvm)
        {
            // Assign ViewData value of 'CompName' for subsequent view
            ViewData["CompName"] = csvm.CompetitionName;

            // Use AddVote() function in CompetitionSubmissionDAL to add one vote to the specific comp submission
            competitionSubmissionContext.AddVote(csvm.CompetitorID, csvm.CompetitionID);

            // Set context value of a specific competition to 1, to indicate that the public member has voted
            HttpContext.Session.SetInt32(csvm.CompetitionID.ToString(), 1);
            return RedirectToAction("ViewCompetitors", new { id = csvm.CompetitionID });
        }

        // Function is called when the comment form is submitted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addComment(Comment comment)
        {
            if (!ModelState.IsValid) // validation fails
            {
                TempData["InputEmpty"] = "Your comment can't be empty!";
                return RedirectToAction("PostComment", "Guest", new { id = comment.CompetitionID }); // returns the view with errors
            }

            // Use Add() function in CommentDAL to add comment to specific competition
            comment.CommentID = commentContext.Add(comment);
            return RedirectToAction("GuestComment", new { id = comment.CompetitionID });
        }

        // The function to convert competition submission into a ViewModel object
        public CompSubmissionViewModel MapTocsVM(CompetitionSubmission comp, int? id)
        {
            string compName = "";
            string competitorName = "";
            string competitorSalutation = "";
            DateTime? startDate = default;
            DateTime? endDate = default;
           
            List<Competition> compList = competitionContext.GetAllCompetitions();
            List<Competitor> competitors = competitorContext.GetAllCompetitors();
           
            foreach (Competition c in compList)
            {
                if (c.CompetitionID == id)
                {
                    compName = c.CompetitionName;
                    startDate = c.StartDate;
                    endDate = c.EndDate;
                    break;
                }
            }

            foreach(Competitor c in competitors)
            {
                if (c.CompetitorID == comp.CompetitorID)
                {
                    competitorName = c.CompetitorName;
                    competitorSalutation = c.Salutation;
                    break;
                }
            }
            CompSubmissionViewModel csVM = new CompSubmissionViewModel
            {
                CompetitionName = compName,
                CompetitorName = competitorName,
                Salutation = competitorSalutation,
                CompetitionID = comp.CompetitionID,
                CompetitorID = comp.CompetitorID,
                FileSubmitted = comp.FileSubmitted,
                DateTimeFileUpload = comp.DateTimeFileUpload,
                Appeal = comp.Appeal,
                VoteCount = comp.VoteCount,
                Ranking = comp.Ranking,    
                StartDate = startDate,
                EndDate = endDate
            };
            return csVM;

        }

        // Function is called when a public member wants to view the final rankings of a completed competition
        public ActionResult FinalRanking(int id)
        {
            List<CompetitionSubmission> competitionSubmissions = competitionSubmissionContext.GetAllSubmissions();
            IEnumerable<CompSubmissionViewModel> topthreelist = new List<CompSubmissionViewModel>();
            List<CompSubmissionViewModel> csVMs = new List<CompSubmissionViewModel>();

            foreach (CompetitionSubmission cs in competitionSubmissions)
            {
                if (cs.CompetitionID == id)
                {
                    csVMs.Add(MapTocsVM(cs, id));
                    topthreelist = csVMs.OrderBy(x => x.Ranking).Take(3);
                }
            }
            return View(topthreelist);
        }

    
    }
}
