using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

//< !--<!-- Edited this 7/7/2021--> 8pm-- Delete Judges Function>

namespace T03_CompetitionPlatform.Models
{
    public class AdminJudgeViewModel
    {
        public List<Judge> judgeList { get; set; }
        public List<AreaInterest> areaList { get; set; }
        public List<CompetitionJudge> compJudgeList { get; set; }
        public List<Competition> compList { get; set; }

        public int CompetitionID { get; set; }
        public int JudgeID { get; set; }

        public AdminJudgeViewModel()
        {
            judgeList = new List<Judge>();
            areaList = new List<AreaInterest>();
            compJudgeList = new List<CompetitionJudge>();
            compList = new List<Competition>();
        }

    }
}