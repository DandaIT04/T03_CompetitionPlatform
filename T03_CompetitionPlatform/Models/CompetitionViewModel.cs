using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace T03_CompetitionPlatform.Models
{
    public class CompetitionViewModel
    {
        public List<Criteria> criteriaList { get; set; }
        public List<Competition> compList { get; set; }

        public CompetitionViewModel()
        {
            criteriaList = new List<Criteria>();
            compList = new List<Competition>();
        }

    }
}
