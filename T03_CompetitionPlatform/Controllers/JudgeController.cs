using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using T03_CompetitionPlatform.DAL;
using T03_CompetitionPlatform.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace T03_CompetitionPlatform.Controllers
{
    public class JudgeController : Controller
    {

        private CompetitionDAL competitionContext = new CompetitionDAL();
        private JudgesDAL judgeContext = new JudgesDAL();
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private CompetitionSubmissionDAL competitionSubmissionContext = new CompetitionSubmissionDAL();
        private CriteriaDAL criteriaContext = new CriteriaDAL();
        private CompetitionJudgeDAL compjudgeContext = new CompetitionJudgeDAL();
        private CompetitionScoreDAL compscoreContext = new CompetitionScoreDAL();
        private AreaInterestDAL areaContext = new AreaInterestDAL();





        // GET: JudgeController
        public ActionResult Index()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }


            //Getting list competitions judge is assigned to
            List<Competition> compList = competitionContext.GetAllCompetitions();

            List<CompetitionJudge> compjudgelist = compjudgeContext.GetAllCompetitionJudge();

            string loggedin = (string)TempData.Peek("Loggedin");

            Judge userlogged = judgeContext.Getlogin(loggedin);

            List<int> compinvolvedID = new List<int>();

            for (int i = 0; i < compjudgelist.Count; i++)
            {
                if (compjudgelist[i].JudgeID == userlogged.JudgeID)
                {
                    compinvolvedID.Add(compjudgelist[i].CompetitionID);
                }
            }

            List<Competition> compinvolvedList = new List<Competition>();


            for (int j = 0; j < compList.Count; j++)
            {
                for (int i = 0; i < compinvolvedID.Count; i++)
                {
                    if (compList[j].CompetitionID == compinvolvedID[i])
                    {
                        compinvolvedList.Add(compList[j]);
                    }

                }
            }



            return View(compinvolvedList);
        }



        // GET: JudgeController/ViewCriteria/5
        public ActionResult ViewCriteria(int? id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }

            CompetitionViewModel compVM = new CompetitionViewModel();
            compVM.compList = competitionContext.GetAllCompetitions();

            if (id != null)
            {
                TempData["CompID"] = id.Value;
                Competition competition = competitionContext.GetDetails(id.Value);
                ViewData["CompName"] = competition.CompetitionName;




                ViewData["selectedCompID"] = id.Value;
                // Get list of Criteria belonging to competition
                compVM.criteriaList = competitionContext.GetCompCriteria(id.Value);
            }
            else
            {
                int newid = (int)TempData.Peek("CompID");
                Competition competition = competitionContext.GetDetails(newid);
                ViewData["CompName"] = competition.CompetitionName;


                ViewData["selectedCompID"] = newid;
                // Get list of Criteria belonging to competition
                compVM.criteriaList = competitionContext.GetCompCriteria(newid);
            };


            return View(compVM);
        }



        // GET: JudgeController/CreateCriteria
        public ActionResult CreateCriteria()
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            int id = (int)TempData.Peek("CompID");


            Competition competition = competitionContext.GetDetails(id);
            ViewData["CompName"] = competition.CompetitionName;

            Criteria criteria = new Criteria();
            //use current competion id for new criteria
            criteria.CompetitionID = id;

            return View(criteria);


        }

        // POST: JudgeController/CreateCriteria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCriteria(Criteria criteria)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }

            int id = (int)TempData.Peek("CompID");
            List<Criteria> compcriteira = competitionContext.GetCompCriteria(id);
            int totalweightage = 0;
            for (int i = 0; i < compcriteira.Count; i++)
            {
                totalweightage += compcriteira[i].Weightage;
            }
            totalweightage = totalweightage + criteria.Weightage;
            if (ModelState.IsValid && totalweightage <= 100)
            {

                if (!criteriaContext.checkIfCriteriaExists(criteria.CriteriaID))
                {
                    //Add criteria record to database
                    criteria.CriteriaID = criteriaContext.Add(criteria);
                    //Redirect user to Judge/ViewCriteria/ view
                    return RedirectToAction("ViewCriteria");
                }
                else
                {
                    return RedirectToAction("ViewCriteria");
                }
                    
            }
            else
            {
                //Auto change the criteria's weightage so that total is still 100
                criteria.Weightage = 100 - (totalweightage - criteria.Weightage);
                //Add criteria record to database
                criteria.CriteriaID = criteriaContext.Add(criteria);
                //Redirect user to Judge/ViewCriteria/ view
                return RedirectToAction("ViewCriteria");
            }

        }

        // GET: JudgeController/Edit/5
        public ActionResult EditCriteria(int? id)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            int compid = (int)TempData.Peek("CompID");


            Competition competition = competitionContext.GetDetails(compid);
            ViewData["CompName"] = competition.CompetitionName;

            //Getting Criteria details
            Criteria criteria = criteriaContext.GetDetails(id.Value);
            criteria.CriteriaID = id.Value;
            if (criteria == null)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }
            return View(criteria);

        }

        // POST: JudgeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCriteria(Criteria criteria)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            int id = (int)TempData.Peek("CompID");
            List<Criteria> compcriteria = competitionContext.GetCompCriteria(id);
            int totalweightage = 0;
            for (int i = 0; i < compcriteria.Count; i++)
            {
                totalweightage += compcriteria[i].Weightage;
            }
            //checking if total weightage is <= 100
            totalweightage = totalweightage - criteriaContext.GetDetails(criteria.CriteriaID).Weightage + criteria.Weightage;
            if (ModelState.IsValid && totalweightage <= 100)
            {
                if (!criteriaContext.checkIfCriteriaExists(criteria.CriteriaID))
                {
                    //Update Criteria record to database
                    criteria.CriteriaID = criteriaContext.Update(criteria);
                    //Redirect user to Staff/Index view
                    return RedirectToAction("ViewCriteria");
                }
                else
                {
                    return RedirectToAction("ViewCriteria");
                }
            }
            else
            {
                //Auto change the criteria's weightage so that total is still 100
                criteria.Weightage = 100 - (totalweightage - criteria.Weightage);
                //Add criteria record to database
                criteria.CriteriaID = criteriaContext.Add(criteria);
                //Redirect user to Judge/ViewCriteria/ view
                return RedirectToAction("ViewCriteria");
            }
        }

        // GET: JudgeController/Delete/5
        public ActionResult DeleteCriteria(int? id)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            int compid = (int)TempData.Peek("CompID");


            Competition competition = competitionContext.GetDetails(compid);
            ViewData["CompName"] = competition.CompetitionName;

            //getting criteria info
            Criteria criteria = criteriaContext.GetDetails(id.Value);
            if (criteria == null)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }
            return View(criteria);
        }

        // POST: JudgeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCriteria(Criteria criteria)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            // Delete the criteria record from database
            criteriaContext.Delete(criteria.CriteriaID);
            return RedirectToAction("ViewCriteria");
        }


        public ActionResult ViewSubmission()
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            int compid = (int)TempData.Peek("CompID");
            List<CompetitionSubmission> compSubmission = competitionSubmissionContext.GetAllSubmissions();
            List<CompetitionSubmission> specifiedSubmission = new List<CompetitionSubmission>();
            // Getting list of Criteria belonging to competition
            foreach (CompetitionSubmission cs in compSubmission)
            {
                if (cs.CompetitionID == compid)
                {
                    specifiedSubmission.Add(cs);
                }
            }

            // Mapping competitor to correct competition submission
            List<Competitor> competitorList = competitorContext.GetAllCompetitors();
            List<CompSubmissionViewModel> currentSubmissions = new List<CompSubmissionViewModel>();
            foreach (Competitor c in competitorList)
            {
                foreach (CompetitionSubmission cs in specifiedSubmission)
                {
                    if (c.CompetitorID == cs.CompetitorID)
                    {
                        CompSubmissionViewModel csVM = MapTocsVM(cs, compid);
                        currentSubmissions.Add(csVM);
                    }
                }
            }

            ViewData["CompName"] = competitionContext.GetDetails(compid).CompetitionName;
            ViewData["CompID"] = compid;





            return View(currentSubmissions);
        }

        public ActionResult ViewCompetitorWork(int? competitorID, int? competitionID)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<CompetitionSubmission> compSubmission = competitionSubmissionContext.GetAllSubmissions();
            foreach (CompetitionSubmission cs in compSubmission)
            {
                if (cs.CompetitorID == competitorID && cs.CompetitionID == competitionID)
                {
                    return View(MapTocsVM(cs, cs.CompetitionID));
                }
            }
            return View();
        }

        public ActionResult ViewScores(int? competitionid, int? competitorid)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<CompetitionScore> scores = compscoreContext.GetAllScore();
            List<CompetitionScore> specifiedscores = new List<CompetitionScore>();
            List<Criteria> criteria = criteriaContext.GetAllCriteria();

            //getting list of scores belonging to selected competitor
            foreach (CompetitionScore s in scores)
            {
                if (s.CompetitorID == competitorid.Value && s.CompetitionID == competitionid.Value)
                {
                    specifiedscores.Add(s);
                }
            }
            //If competitor does not have any scores in the database
            if (specifiedscores.Count == 0)
            {
                foreach (Criteria c in criteria)
                {
                    if (c.CompetitionID == competitionid.Value)
                    {
                        CompetitionScore cs = new CompetitionScore();
                        cs.CriteriaID = c.CriteriaID;
                        cs.CompetitorID = competitorid.Value;
                        cs.CompetitionID = competitionid.Value;
                        cs.Score = 0;



                        if(!compscoreContext.checkIfJudgeExists(cs.CompetitorID, cs.CriteriaID))
                        {
                            specifiedscores.Add(cs);
                            compscoreContext.Add(cs);
                        }
                    }
                }
            }


            //Maping scores to correct Criteria
            List<CompetitionScoreViewModel> currentScore = new List<CompetitionScoreViewModel>();
            foreach (Criteria c in criteria)
            {
                foreach(CompetitionScore ss in specifiedscores)
                {
                    if (c.CriteriaID == ss.CriteriaID)
                    {
                        CompetitionScoreViewModel csVM = MapToScoreVM(ss);
                        currentScore.Add(csVM);
                    }
                }
            }

            ViewData["Compend"] = competitionContext.GetDetails(competitionid.Value).EndDate;
            ViewData["CompName"] = competitionContext.GetDetails(competitionid.Value).CompetitionName;
            ViewData["CompetitorName"] = competitorContext.GetDetails(competitorid.Value).Salutation + " " + competitorContext.GetDetails(competitorid.Value).CompetitorName;
            

            return View(currentScore);
        }

        public ActionResult GradeCriterion(int? criteriaid, int? competitorid, string criterianame, int? weightage)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            CompetitionScore score = compscoreContext.GetDetails(criteriaid.Value, competitorid.Value);
            




            ViewData["CompetitorName"] = competitorContext.GetDetails(competitorid.Value).Salutation + " " + competitorContext.GetDetails(competitorid.Value).CompetitorName;
            ViewData["CriteriaName"] = criterianame;
            ViewData["Weightage"] = weightage.Value;

            return View(score);
        }

        [HttpPost]
        public ActionResult GradeCriterion(CompetitionScore updatescore)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                //Update score record to database
                compscoreContext.Update(updatescore);
                //Redirect user to Judge/ViewScores view
                return RedirectToAction("ViewScores", new { competitionid = updatescore.CompetitionID, competitorid = updatescore.CompetitorID });
            }
            else
            {
                return RedirectToAction("GradeCriterion", new { competitionid = updatescore.CompetitionID, competitorid = updatescore.CompetitorID });
            }
        }

        public ActionResult Rank(int? id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }
            //Get all the participating CompetitionSubmission
            List<CompetitionSubmission> compSubmission = competitionSubmissionContext.GetAllSubmissions();
            List<CompetitionSubmission> specifiedSubmission = new List<CompetitionSubmission>();
            foreach (CompetitionSubmission cs in compSubmission)
            {
                if (cs.CompetitionID == id)
                {
                    specifiedSubmission.Add(cs);
                }
            }


            List<Competitor> competitorList = competitorContext.GetAllCompetitors();
            List<CompSubmissionViewModel> currentSubmissions = new List<CompSubmissionViewModel>();
            foreach (Competitor c in competitorList)
            {
                foreach (CompetitionSubmission cs in specifiedSubmission)
                {
                    if (c.CompetitorID == cs.CompetitorID)
                    {
                        CompSubmissionViewModel csVM = MapTocsVM(cs, id);
                        currentSubmissions.Add(csVM);
                    }
                }
            }

            currentSubmissions = currentSubmissions.OrderBy(o => o.Ranking).ToList();

            // to calculate total marks for each submission (for judges only) 
            
            List <double> totalmarks = new List<double>();
            
            List<CompetitionScore> scores = compscoreContext.GetAllScore();

            List<CompetitionScore> specifiedscores = new List<CompetitionScore>();
            List<Criteria> criteria = criteriaContext.GetAllCriteria();

            //geting all the participating competition score view models
            foreach (CompetitionScore s in scores)
            {
                if (s.CompetitionID == id)
                {
                    specifiedscores.Add(s);
                }
            }

            List<CompetitionScoreViewModel> currentScore = new List<CompetitionScoreViewModel>();
            foreach (Criteria c in criteria)
            {
                foreach (CompetitionScore ss in specifiedscores)
                {
                    if (c.CriteriaID == ss.CriteriaID)
                    {
                        CompetitionScoreViewModel csVM = MapToScoreVM(ss);
                        currentScore.Add(csVM);
                    }
                }
            }
            

            
            foreach (CompSubmissionViewModel ss in currentSubmissions)
            {
                double marks = 0;
                
                foreach (CompetitionScoreViewModel cs in currentScore)
                {
                    if (ss.CompetitorID == cs.CompetitorID)
                    {
                        double realscore = cs.Score;

                        marks = marks + (cs.Weightage * (realscore / 10));
                        
                    }
                    
                    
                }
                totalmarks.Add(marks);




            }
                
                

            
            ViewData["totalmarks"] = totalmarks;
            ViewData["CompName"] = competitionContext.GetDetails(id.Value).CompetitionName;
            ViewData["CompEnd"] = competitionContext.GetDetails(id.Value).EndDate;






            return View(currentSubmissions);
        }

        [HttpPost]
        public ActionResult Rank(List<CompSubmissionViewModel> compsubList)
        {

            // Stop accessing the action if not logged in
            // or account not in the "Judge" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }

            //Get all the participating CompetitionSubmission
            List<CompetitionSubmission> compSubmission = competitionSubmissionContext.GetAllSubmissions();
            List<CompetitionSubmission> specifiedSubmission = new List<CompetitionSubmission>();
            foreach (CompetitionSubmission cs in compSubmission)
            {
                if (cs.CompetitionID == compsubList[0].CompetitionID)
                {
                    specifiedSubmission.Add(cs);
                }
            }


            List<Competitor> competitorList = competitorContext.GetAllCompetitors();
            List<CompSubmissionViewModel> currentSubmissions = new List<CompSubmissionViewModel>();
            foreach (Competitor c in competitorList)
            {
                foreach (CompetitionSubmission cs in specifiedSubmission)
                {
                    if (c.CompetitorID == cs.CompetitorID)
                    {
                        CompSubmissionViewModel csVM = MapTocsVM(cs, compsubList[0].CompetitionID);
                        currentSubmissions.Add(csVM);
                    }
                }
            }

            currentSubmissions = currentSubmissions.OrderBy(o => o.Ranking).ToList();

            // to calculate total marks for each submission (for judges only) 

            List<double> totalmarks = new List<double>();

            List<CompetitionScore> scores = compscoreContext.GetAllScore();

            List<CompetitionScore> specifiedscores = new List<CompetitionScore>();
            List<Criteria> criteria = criteriaContext.GetAllCriteria();

            //geting all the participating competition score view models
            foreach (CompetitionScore s in scores)
            {
                if (s.CompetitionID == compsubList[0].CompetitionID)
                {
                    specifiedscores.Add(s);
                }
            }

            List<CompetitionScoreViewModel> currentScore = new List<CompetitionScoreViewModel>();
            foreach (Criteria c in criteria)
            {
                foreach (CompetitionScore ss in specifiedscores)
                {
                    if (c.CriteriaID == ss.CriteriaID)
                    {
                        CompetitionScoreViewModel csVM = MapToScoreVM(ss);
                        currentScore.Add(csVM);
                    }
                }
            }



            foreach (CompSubmissionViewModel ss in currentSubmissions)
            {
                double marks = 0;

                foreach (CompetitionScoreViewModel cs in currentScore)
                {
                    if (ss.CompetitorID == cs.CompetitorID)
                    {
                        double realscore = cs.Score;

                        marks = marks + (cs.Weightage * (realscore / 10));

                    }


                }
                totalmarks.Add(marks);




            }

            //Removing competition submissions with a null ranking value after updating it to the database
            List<CompetitionSubmission> csList = new List<CompetitionSubmission>();

            for (int i = 0; i < compsubList.Count; i++)
            {
                Debug.WriteLine(compsubList.Count);
                

                if (compsubList[i].Ranking == null)
                {
                    Debug.WriteLine("hi" + compsubList[i].CompetitorID);

                    CompetitionSubmission compsub = new CompetitionSubmission();
                    compsub.Appeal = compsubList[i].Appeal;
                    compsub.CompetitionID = compsubList[i].CompetitionID;
                    compsub.CompetitorID = compsubList[i].CompetitorID;
                    compsub.DateTimeFileUpload = compsubList[i].DateTimeFileUpload;
                    compsub.FileSubmitted = compsubList[i].FileSubmitted;
                    compsub.Ranking = compsubList[i].Ranking;
                    compsub.VoteCount = compsubList[i].VoteCount;
                    competitionSubmissionContext.UpdateRank(compsub);
                    compsubList.RemoveAt(i);
                    i = i - 1;

                }
                if(compsubList.Count == 0){
                    return RedirectToAction("ViewCompetitors", "Guest", new { id = TempData.Peek("CompID") });
                }
            }

            //getting List of all the remaining competition submissions
            foreach (CompSubmissionViewModel csvm in compsubList)
            {
                CompetitionSubmission compsub = new CompetitionSubmission();
                compsub.Appeal = csvm.Appeal;
                compsub.CompetitionID = csvm.CompetitionID;
                compsub.CompetitorID = csvm.CompetitorID;
                compsub.DateTimeFileUpload = csvm.DateTimeFileUpload;
                compsub.FileSubmitted = csvm.FileSubmitted;
                compsub.Ranking = csvm.Ranking;
                compsub.VoteCount = csvm.VoteCount;

                csList.Add(compsub);
            }


            //Sorting the List in descending order of Ranking value
            csList = csList.OrderBy(o => o.Ranking).ToList();

            //Validation for if Ranking values are not consecutive
            for (int i= 0; i < csList.Count; i++)
            {
                
                
                   csList[i].Ranking = i + 1;
                

                
            }




            foreach (CompetitionSubmission cs in csList)
            {
                competitionSubmissionContext.UpdateRank(cs);
            }

            return RedirectToAction("ViewCompetitors", "Guest", new { id = TempData.Peek("CompID") });
        }

        public ActionResult judgeRegister()
        {
            List<SelectListItem> aoiList = new List<SelectListItem>();

            foreach (var item in GetAllAOI())
            {
                aoiList.Add(new SelectListItem
                {
                    Value = Convert.ToString(item.AreaInterestID),
                    Text = item.Name


                }); ;
            }

            ViewData["AOIList"] = aoiList;
            ViewData["SalutationList"] = GetSalutations();



            return View();
        }

        [HttpPost]
        public ActionResult judgeRegister(Judge judge)
        {
            ViewData["AOIList"] = GetAllAOI();

            if (ModelState.IsValid)
            {
                //Add Judge record to database
                judge.JudgeID = judgeContext.Add(judge);
                //Redirect user to Home/Index view
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Input validation fails, return to the Create view
                //to display error message


                return View(judge);
            }
        }

        private List<AreaInterest> GetAllAOI()
        {


            // Get a list of Area of interest from database
            List<AreaInterest> aoiList = areaContext.GetAllArea();
            

            return aoiList;
        }



        






        public CompSubmissionViewModel MapTocsVM(CompetitionSubmission comp, int? id)
        {
            string compName = "";
            string competitorName = "";
            string competitorSalutation = "";

            List<Competition> compList = competitionContext.GetAllCompetitions();
            List<Competitor> competitors = competitorContext.GetAllCompetitors();

            foreach (Competition c in compList)
            {
                if (c.CompetitionID == id)
                {
                    compName = c.CompetitionName;
                    break;
                }
            }

            foreach (Competitor c in competitors)
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
            };
            return csVM;

        }
        public CompetitionScoreViewModel MapToScoreVM(CompetitionScore score)
        {
            string criterianame = "";
            int weightage = 0;

            List<Criteria> criList = criteriaContext.GetAllCriteria();
            foreach (Criteria criteria in criList)
            {
                if (criteria.CriteriaID == score.CriteriaID)
                {


                    criterianame = criteria.CriteriaName;
                    weightage = criteria.Weightage;
                    //Exit the foreach loop once the name is found
                    break;
                }
            }


            CompetitionScoreViewModel scoreVM = new CompetitionScoreViewModel
            {
                CriteriaID = score.CriteriaID,
                CompetitorID = score.CompetitorID,
                CompetitionID = score.CompetitionID,
                CriteriaName = criterianame,
                Weightage = weightage,
                Score = score.Score
            };
            return scoreVM;
        }
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
    }
}
