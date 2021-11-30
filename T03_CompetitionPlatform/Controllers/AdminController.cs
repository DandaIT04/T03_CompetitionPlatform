using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T03_CompetitionPlatform.DAL;
using T03_CompetitionPlatform.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

//< !--Edite<!-- Edited this 7/7/2021-->-- Delete Judges Function>

namespace T03_CompetitionPlatform.Controllers
{
    public class AdminController : Controller
    {
        // Execute functions from DAL e.g areaContext.GetAllArea()
        private AreaInterestDAL areaContext = new AreaInterestDAL();
        private CompetitionDAL competitionContext = new CompetitionDAL();
        private JudgesDAL judgeContext = new JudgesDAL();
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private CompetitionSubmissionDAL competitorSubmissionContext = new CompetitionSubmissionDAL();
        private CompetitionJudgeDAL competitionJudgeContext = new CompetitionJudgeDAL();


        // GET: AdminController
        public ActionResult Index()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            // Return area list
            List<AreaInterest> areaList = areaContext.GetAllArea();
            return View(areaList);
        }

        public ActionResult AdminMain()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            // Get count of totala areas
            List<AreaInterest> areaList = areaContext.GetAllArea();

            int count = 0;
            foreach (AreaInterest area in areaList)
            {
                count += 1;
            }

            TempData["areaCount"] = Convert.ToString(count);

            // Get count of total competitions
            List<Competition> competitionList = competitionContext.GetAllCompetitions();

            int count1 = 0;
            foreach (Competition competitions in competitionList)
            {
                count1 += 1;
            }

            TempData["competitionCount"] = Convert.ToString(count1);

            // Get count of total judge
            List<Judge> judgeList = judgeContext.GetAllJudges();

            int count2 = 0;
            foreach (Judge judges in judgeList)
            {
                count2 += 1;
            }

            TempData["judgesCount"] = Convert.ToString(count2);

            // Get count of total competitors
            List<Competitor> competitorsList = competitorContext.GetAllCompetitors();

            int count3 = 0;
            foreach (Competitor competitors in competitorsList)
            {
                count3 += 1;
            }


            // All TempData counts is to be called in view to display total amount
            TempData["competitorsCount"] = Convert.ToString(count3);



            return View("/Views/Admin/AdminMain.cshtml");


        }

        public ActionResult CompetitionRecordsView()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            // Gets competitions list for the view
            List<Competition> competitionList = competitionContext.GetAllCompetitions();
            return View(competitionList);
        }

        public ActionResult ManageJudgesView()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }


            // Basically a new model is created which stores data contained in other models
            // That can be used for ManageJudgesView to retrieve details like CompetitionName
            // When the main focus is CompetitionJudgeDAL
            AdminJudgeViewModel adminJudgeVM = new AdminJudgeViewModel();
            adminJudgeVM.judgeList = judgeContext.GetAllJudges();
            adminJudgeVM.areaList = areaContext.GetAllArea();
            adminJudgeVM.compJudgeList = competitionJudgeContext.GetAllCompetitionJudge();
            adminJudgeVM.compList = competitionContext.GetAllCompetitions();

            return View(adminJudgeVM);
        }

        // POST: StaffController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageJudgesView(AdminJudgeViewModel selectedJudge)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            // Assign selectedJudge values to int variables to be checked
            // Basically convert AdminJudgeViewModel values to int and convert
            // Them to CompetitionJudge
            int selectComp = selectedJudge.CompetitionID;
            int selectJudge = selectedJudge.JudgeID;

            CompetitionJudge newcJudge = new CompetitionJudge();

            // Assign newcJudge values with the int variable values
            newcJudge.CompetitionID = selectComp;
            newcJudge.JudgeID = selectJudge;

            if (ModelState.IsValid)
            {
                if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }

                // Validations
                List<Judge> judgeList = judgeContext.GetAllJudges();
                List<CompetitionJudge> compJustList = competitionJudgeContext.GetAllCompetitionJudge();
                List<Competition> competitionList = competitionContext.GetAllCompetitions();

                // If judge not found, tempdata will be executed
                bool judgeFound = false;
                foreach (Judge theJudges in judgeList)
                {
                    if (theJudges.JudgeID == newcJudge.JudgeID)
                    {
                        judgeFound = true;
                    }
                }

                if (judgeFound == false)
                {
                    TempData["JudgeNull"] = "JudgeID does not exists!";
                    return RedirectToAction("ManageJudgesView", "Admin");
                }



                foreach (CompetitionJudge compJust in compJustList)
                {
                    // Check if data row already exists
                    if (newcJudge.CompetitionID == compJust.CompetitionID && newcJudge.JudgeID == compJust.JudgeID)
                    {
                        TempData["Duplicate"] = "Duplicate row!";
                        return RedirectToAction("ManageJudgesView", "Admin");
                    }

                    // check if judgeid exists in competitionjudge table
                    if (newcJudge.JudgeID == compJust.JudgeID)
                    {
                        foreach (Competition competitions in competitionList)
                        {
                            if (competitions.CompetitionID == newcJudge.CompetitionID)
                            {
                                DateTime now = DateTime.Now;

                                // Current or future competitions right?
                                if (competitions.EndDate > now)
                                {
                                    TempData["AlreadyAssigned"] = "Judge already assigned to competition that is ongoing!";
                                    return RedirectToAction("ManageJudgesView", "Admin");
                                }

                                else if (competitions.StartDate > now)
                                {
                                    TempData["AlreadyAssigned"] = "Judge already assigned to competition that is ongoing!";
                                    return RedirectToAction("ManageJudgesView", "Admin");
                                }

                                else if (competitions.StartDate == null)
                                {
                                    TempData["AlreadyAssigned"] = "Judge already assigned to competition that is ongoing!";
                                    return RedirectToAction("ManageJudgesView", "Admin");
                                }
                            }
                        }
                    }
                }

                bool compFound = false;
                foreach (Competition competitionCheck in competitionList)
                {
                    // check if competition exists
                    if (competitionCheck.CompetitionID == newcJudge.CompetitionID)
                    {
                        compFound = true;
                    }

                    // Steps to check if competition already ended
                    if (newcJudge.CompetitionID == competitionCheck.CompetitionID)
                    {
                        DateTime theDate = DateTime.Now;
                        if (theDate > competitionCheck.EndDate)
                        {
                            TempData["EndDateExceeded"] = "Judge cannot be assigned to a competition past end date!";
                            return RedirectToAction("ManageJudgesView", "Admin");
                        }
                    }
                }

                // If competition not found
                if (compFound == false)
                {
                    TempData["CompetitionNull"] = "CompetitionID does not exists!";
                    return RedirectToAction("ManageJudgesView", "Admin");
                }

                // Check if competition is in the same area of interest as judge
                foreach (Competition competitions in competitionList)
                {
                    int theNo = 0;

                    if (competitions.CompetitionID == newcJudge.CompetitionID)
                    {
                        theNo = competitions.AreaInterestID;

                        foreach (Judge judges in judgeList)
                        {
                            if (judges.JudgeID == newcJudge.JudgeID && judges.AreaInterestID != theNo)
                            {
                                string judgeID = Convert.ToString(newcJudge.JudgeID);
                                string areaID = Convert.ToString(judges.AreaInterestID);

                                TempData["WrongInterest"] = "The competition falls under the wrong area of interest for the judge!" + "JudgeID: " + judgeID + "AreaID: " + areaID;
                                return RedirectToAction("ManageJudgesView", "Admin");
                            }
                        }
                    }
                }


                ViewData["CompetitionList"] = competitionList;

                //Update staff record to database
                competitionJudgeContext.AddJudges(newcJudge);
                return RedirectToAction("ManageJudgesView", "Admin");
            }
            else
            {
                //Input validation fails, return to the view
                //to display error message

                TempData["MissingInput1"] = "Inputs cannot be empty!";
                return RedirectToAction("ManageJudgesView", "Admin");
            }
        }

        public ActionResult CreateAreaView()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
(HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View("/Views/Admin/CreateAreaView.cshtml");
        }

        public ActionResult CreateCompetitionsView()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
(HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            // Get area and competitions to adminCreateVM to retrieve data from two different tables
            AdminCreateCompDisplayArea adminCreateVM = new AdminCreateCompDisplayArea();
            adminCreateVM.areaList = areaContext.GetAllArea();
            adminCreateVM.compList = competitionContext.GetAllCompetitions();

            return View(adminCreateVM);
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateArea(AreaInterest area)
        {
            if (ModelState.IsValid)
            {
                if ((HttpContext.Session.GetString("Role") == null) ||
(HttpContext.Session.GetString("Role") != "Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }

                //Add staff record to database
                area.AreaInterestID = areaContext.Add(area);
                //Redirect user to Staff/Index view
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View("/Views/Admin/CreateAreaView.cshtml", area);
            }

        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCompetition(Competition competitions)
        {
            if (ModelState.IsValid)
            {
                if ((HttpContext.Session.GetString("Role") == null) ||
(HttpContext.Session.GetString("Role") != "Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }
                // Check if area ID exists
                List<AreaInterest> areaList = areaContext.GetAllArea();

                bool checkIfAreaIdExists = false;

                foreach (AreaInterest area in areaList)
                {
                    if (competitions.AreaInterestID == area.AreaInterestID)
                    {
                        checkIfAreaIdExists = true;
                    }
                }

                if (checkIfAreaIdExists == false)
                {
                    TempData["AreaIDFalse"] = "AreaID does not exist!";

                    return RedirectToAction("CreateCompetitionsView", "Admin");
                }

                // basic checks
                if (competitions.AreaInterestID == null)
                {
                    TempData["IDnull"] = "Input AreaID!";

                    return RedirectToAction("CreateCompetitionsView", "Admin");
                }

                if (competitions.CompetitionName == null)
                {
                    TempData["NameNull"] = "Input CompetitionName!";

                    return RedirectToAction("CreateCompetitionsView", "Admin");
                }

                //Perform checks on dates

                if (competitions.StartDate != null || competitions.EndDate != null || competitions.ResultReleasedDate != null)
                {
                    DateTime theDate = DateTime.Now;

                    if (competitions.StartDate != null)
                    {
                        if (competitions.StartDate <= theDate)
                        {
                            TempData["TodayDateError"] = "StartDate cannot be set before today's Date!";

                            return RedirectToAction("CreateCompetitionsView", "Admin");
                        }
                    }

                    if (competitions.EndDate != null && competitions.StartDate != null)
                    {
                        if (competitions.StartDate >= competitions.EndDate)
                        {
                            TempData["StartDate>EndDate"] = "StartDate cannot be set after EndDate!";

                            return RedirectToAction("CreateCompetitionsView", "Admin");
                        }

                        else if (competitions.EndDate <= competitions.StartDate)
                        {
                            TempData["StartDate<EndDate"] = "EndDate cannot be set before StartDate!";

                            return RedirectToAction("CreateCompetitionsView", "Admin");
                        }

                        if (competitions.ResultReleasedDate != null)
                        {
                            if (competitions.ResultReleasedDate <= competitions.StartDate)
                            {
                                TempData["ResultDate<StartDate"] = "ResultDate Cannot be before Start Date!";

                                return RedirectToAction("CreateCompetitionsView", "Admin");
                            }

                            if (competitions.ResultReleasedDate <= competitions.EndDate)
                            {
                                TempData["ResultDate<EndDate"] = "ResultDate Cannot be before End Date!";

                                return RedirectToAction("CreateCompetitionsView", "Admin");
                            }
                        }

                    }

                    if (competitions.ResultReleasedDate != null && competitions.StartDate != null ||
                        competitions.ResultReleasedDate != null && competitions.EndDate != null)
                    {
                        if (competitions.ResultReleasedDate <= competitions.StartDate)
                        {
                            TempData["ResultDate<StartDate"] = "ResultDate Cannot be before Start Date!";

                            return RedirectToAction("CreateCompetitionsView", "Admin");
                        }

                        if (competitions.ResultReleasedDate <= competitions.EndDate)
                        {
                            TempData["ResultDate<EndDate"] = "ResultDate Cannot be before End Date!";

                            return RedirectToAction("CreateCompetitionsView", "Admin");
                        }
                    }

                    if (competitions.ResultReleasedDate != null && competitions.StartDate != null && competitions.EndDate == null)
                    {
                        TempData["EndDateMissing"] = "EndDate is missing!";

                        return RedirectToAction("CreateCompetitionsView", "Admin");
                    }
                }


                //Add staff record to database
                competitions.CompetitionID = competitionContext.Add(competitions);
                //Redirect user to Staff/Index view
                return RedirectToAction("CompetitionRecordsView", "Admin");
            }
            else
            {
                // check if Competition Name already exist
                List<Competition> competitionList = competitionContext.GetAllCompetitions();


                bool checkIfCompetitionNameExists = false;

                foreach (Competition competitionsInList in competitionList)
                {
                    if (competitions.CompetitionName == competitionsInList.CompetitionName)
                    {
                        checkIfCompetitionNameExists = true;
                    }
                }

                if (checkIfCompetitionNameExists == true)
                {
                    TempData["CompetitionNameExist"] = "Competition name already exists!";

                    return RedirectToAction("CreateCompetitionsView", "Admin");
                }

                else
                {
                    TempData["InputEmpty"] = "AreaofInterestID and CompetitionName CANNOT be empty!";
                    //Input validation fails, return to the Create view
                    //to display error message
                    return RedirectToAction("CreateCompetitionsView", "Admin");
                }
            }

        }

        // GET: AdminController/Delete/5
        public ActionResult DeleteArea(int? id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index", "Admin");
            }

            AreaInterest area = areaContext.GetDetails(id.Value);

            if (area == null)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index", "Admin");
            }

            // check if area has assigned values
            if (id != null)
            {
                int theId = Convert.ToInt32(id);

                bool checkCompetitions = competitionContext.checkIfCompetitionExists(theId);

                bool checkJudges = judgeContext.checkIfJudgeExists(theId);

                if (checkCompetitions == true || checkJudges == true)
                {
                    TempData["AreaDeleteMessage"] = "Area has competition(s)! or judge(s) assigned!";

                    return RedirectToAction("Index", "Admin");
                }
            }



            return View(area);
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteArea(AreaInterest area)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
(HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            // Delete the area record from database
            areaContext.DeleteArea(area.AreaInterestID);
            return RedirectToAction("Index", "Admin");
        }

        // GET: AdminController/Delete/5
        public ActionResult DeleteCompetition(int? id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index", "Admin");
            }

            Competition competition = competitionContext.GetDetails(id.Value);

            if (competition == null)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index", "Admin");
            }

            // check if competitions has competitors assigned
            if (id != null)
            {
                int theId = Convert.ToInt32(id);

                bool checkSubmissions = competitorSubmissionContext.checkIfCompetitorSubmissionExists(theId);

                if (checkSubmissions == true)
                {
                    TempData["CompetitionDeleteMessage"] = "Competition has competitor(s)!";

                    return RedirectToAction("CompetitionRecordsView", "Admin");
                }
            }



            return View(competition);
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCompetition(Competition competition)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
(HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            // Delete the area record from database
            competitionContext.DeleteCompetition(competition.CompetitionID);
            return RedirectToAction("CompetitionRecordsView", "Admin");
        }

        // GET: StaffController/Edit/5
        public ActionResult EditCompetition(int? id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }

            // get the details of competition to be edited
            Competition competitions = competitionContext.GetDetails(id.Value);

            if (competitions == null)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }
            return View(competitions);
        }

        // POST: StaffController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCompetition(Competition competitions)
        {

            if (ModelState.IsValid)
            {
                if ((HttpContext.Session.GetString("Role") == null) ||
(HttpContext.Session.GetString("Role") != "Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }

                if (competitions.CompetitionName == null || competitions.CompetitionName == "")
                {
                    TempData["NameMissing"] = "Competition name is empty!";
                    return RedirectToAction("EditCompetition", "Admin");
                }

                //Perform checks on dates

                if (competitions.StartDate != null || competitions.EndDate != null || competitions.ResultReleasedDate != null)
                {

                    if (competitions.EndDate != null && competitions.StartDate != null)
                    {
                        if (competitions.StartDate >= competitions.EndDate)
                        {
                            TempData["StartDate>EndDate"] = "StartDate cannot be set after EndDate!";

                            return RedirectToAction("EditCompetition", "Admin");
                        }

                        else if (competitions.EndDate <= competitions.StartDate)
                        {
                            TempData["StartDate<EndDate"] = "EndDate cannot be set before StartDate!";

                            return RedirectToAction("EditCompetition", "Admin");
                        }

                        if (competitions.ResultReleasedDate != null)
                        {
                            if (competitions.ResultReleasedDate <= competitions.StartDate)
                            {
                                TempData["ResultDate<StartDate"] = "ResultDate Cannot be before Start Date!";

                                return RedirectToAction("EditCompetition", "Admin");
                            }

                            if (competitions.ResultReleasedDate <= competitions.EndDate)
                            {
                                TempData["ResultDate<EndDate"] = "ResultDate Cannot be before End Date!";

                                return RedirectToAction("EditCompetition", "Admin");
                            }
                        }

                    }

                    if (competitions.ResultReleasedDate != null && competitions.StartDate != null ||
                        competitions.ResultReleasedDate != null && competitions.EndDate != null)
                    {
                        if (competitions.ResultReleasedDate <= competitions.StartDate)
                        {
                            TempData["ResultDate<StartDate"] = "ResultDate Cannot be before Start Date!";

                            return RedirectToAction("EditCompetition", "Admin");
                        }

                        if (competitions.ResultReleasedDate <= competitions.EndDate)
                        {
                            TempData["ResultDate<EndDate"] = "ResultDate Cannot be before End Date!";

                            return RedirectToAction("EditCompetition", "Admin");
                        }
                    }

                    if (competitions.ResultReleasedDate != null && competitions.StartDate != null && competitions.EndDate == null)
                    {
                        TempData["EndDateMissing"] = "EndDate is missing!";

                        return RedirectToAction("EditCompetition", "Admin");
                    }
                }

                //Update staff record to database
                competitionContext.Update(competitions);
                return RedirectToAction("CompetitionRecordsView", "Admin");
            }
            else
            {
                if (competitions.CompetitionName == null || competitions.CompetitionName == "")
                {
                    TempData["NameMissing"] = "Competition name is empty!";
                    return RedirectToAction("EditCompetition", "Admin");
                }
                //Input validation fails, return to the view
                //to display error message
                return RedirectToAction("EditCompetition", "Admin");
            }
        }

        public ActionResult RemoveJudges()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            // Use AdminJudgeViewModel to retrieve multiple different values from different models/dal
            AdminJudgeViewModel adminJudgeVM = new AdminJudgeViewModel();
            adminJudgeVM.judgeList = judgeContext.GetAllJudges();
            adminJudgeVM.areaList = areaContext.GetAllArea();
            adminJudgeVM.compJudgeList = competitionJudgeContext.GetAllCompetitionJudge();
            adminJudgeVM.compList = competitionContext.GetAllCompetitions();

            return View(adminJudgeVM);
        }

        // POST: StaffController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveJudges(CompetitionJudge compJudge)
        {
            if (ModelState.IsValid)
            {
                if ((HttpContext.Session.GetString("Role") == null) ||
(HttpContext.Session.GetString("Role") != "Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }

                // Validations
                List<Judge> judgeList = judgeContext.GetAllJudges();
                List<CompetitionJudge> compJustList = competitionJudgeContext.GetAllCompetitionJudge();
                List<Competition> competitionList = competitionContext.GetAllCompetitions();

                bool judgeFound = false;
                foreach (Judge theJudges in judgeList)
                {
                    if (theJudges.JudgeID == compJudge.JudgeID)
                    {
                        judgeFound = true;
                    }
                }

                if (judgeFound == false)
                {
                    TempData["JudgeNull"] = "JudgeID does not exists!";
                    return RedirectToAction("RemoveJudges", "Admin");
                }


                bool compFound = false;
                foreach (Competition competitionCheck in competitionList)
                {
                    if (competitionCheck.CompetitionID == compJudge.CompetitionID)
                    {
                        compFound = true;
                    }

                }



                if (compFound == false)
                {
                    TempData["CompetitionNull"] = "CompetitionID does not exists!";
                    return RedirectToAction("RemoveJudges", "Admin");
                }

                bool doesItExist = false;
                foreach (CompetitionJudge compJust in compJustList)
                {
                    if (compJudge.CompetitionID == compJust.CompetitionID && compJudge.JudgeID == compJust.JudgeID)
                    {
                        doesItExist = true;
                        competitionJudgeContext.RemoveJudges(compJudge);
                    }

                }

                if (doesItExist == false)
                {
                    TempData["RecordNotFound"] = "RecordNotFound!";
                    return RedirectToAction("RemoveJudges", "Admin");
                }

                return RedirectToAction("RemoveJudges", "Admin");
            }
            else
            {
                //Input validation fails, return to the view
                //to display error message

                TempData["MissingInput1"] = "Inputs cannot be empty!";
                return RedirectToAction("RemoveJudges", "Admin");
            }


        }


        public ActionResult AreasOfInterests()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
(HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View("~/Views/Admin/AreasOfInterests.cshtml");
        }
    }
}
