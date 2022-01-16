using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColorsData.Models;

namespace Cards.Models
{
    public class TextBox
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Enter valid Title")]
        public string Title { get; set; }


        [Display(Name = "Type")]
        [Required(ErrorMessage = "Enter valid Type")]
        public MTextBox.TextBoxType Type { get; set; }

        public List<TextBoxElement> Elements { get; set; }

        public string TypeToClass()
        {
            switch (Type)
            {
                case MTextBox.TextBoxType.Problem:
                    return "problem";
                case MTextBox.TextBoxType.Checkpoints:
                    return "checkpoints";
                case MTextBox.TextBoxType.Solution:
                    return "solutions";
                default:
                    return "";
            }
        }
    }
}
