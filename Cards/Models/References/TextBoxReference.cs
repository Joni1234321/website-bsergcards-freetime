using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Models.References
{
    public class TextBoxReference
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }

        public int TextBoxIndex { get; set; }

        public TextBox TextBox { get; set; }
    }
}
