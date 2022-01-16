using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColorsData.Models;

namespace Cards.Models
{
    public class TextBoxElement
    {
        public TextBox TextBox { get; set; }

        public int Id { get; set; }
        public string Content { get; set; }
        public MTextBoxElement.ElementType Type { get; set; }

        public string getImgPath()
        {
            return Content.Split(',')[0];
        }
        public string getImgCaption()
        {
            string[] s = Content.Split(',');
            return s.Length < 2 ? "" : s[1];
        }
    }
}
