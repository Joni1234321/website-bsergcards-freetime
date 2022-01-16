using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ColorsData.Models;

namespace Cards.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Enter valid Title")]
        public string Title { get; set; }


        [Display(Name = "Status")]
        [Required(ErrorMessage = "Enter valid Status")]
        public MProject.Type Status { get; set; }


        [Display(Name = "Tags")]
        [Required(ErrorMessage = "Enter valid Tags")]
        public string[] Tags { get; set; }

        public string StatusToClass()
        {
            switch (Status)
            {
                case MProject.Type.Active:
                    return "active";
                case MProject.Type.Finished:
                    return "finished";
                case MProject.Type.Learning:
                    return "todo";
                default:
                    return "";
            }
        }

        public List<TextBox> TextBoxes { get; set; }
    }
}
