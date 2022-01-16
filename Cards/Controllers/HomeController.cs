using Cards.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ColorsData.Logic;
using Cards.Models.References;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cards.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment Environment;

        public HomeController(IWebHostEnvironment Environment)
        {
            this.Environment = Environment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ViewProject()
        {
            return View();
        }

        [HttpGet("Project/{id}")]
        public IActionResult Project(int id)
        {
            var mProject = DBProject.Load(id);
            if (mProject == null) return View();
            return View(GetProject(mProject));
        }


        public IActionResult ChangeProject(int id)
        {
            var mProject = DBProject.Load(id);
            if (mProject == null) return View("Error404", "Project with ID [" + id + "]");
            return View("ChangeProject", GetProject(mProject));
        }

        public IActionResult ChangeTextBox(int id, int n)
        {
            var mProject = DBProject.Load(id);
            if (mProject == null) return View("Error404", "Project with ID [" + id + "]");

            Project project = GetProject(mProject);
            if (project == null) return View("Error404", "Project with ID (conversion) [" + id + "]");

            if (n >= project.TextBoxes.Count) return View("Error404", "TextBox at index [" + n + "]");

            return View("ChangeTextBox", new TextBoxReference { ProjectId = id, ProjectTitle = project.Title, TextBoxIndex = n, TextBox = project.TextBoxes[n] });
        }


        [HttpGet("Change/{id}/{n}")]
        public IActionResult Change(int id, int n)
        {
            return ChangeTextBox(id, n);
        }
        [HttpGet("Change/{id}")]
        public IActionResult Change(int id)
        {
            return ChangeProject(id);
        }

        #region Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProject(Project project)
        {
            if (ModelState.IsValid)
            {
                if (project.Title == "delete")
                {
                    Project p = GetProject(DBProject.Load(project.Id));
                    if (p.TextBoxes.Count == 0)
                    {
                        DBProject.Delete(project.Id);
                        return RedirectToAction("Projects");
                    }
                    project.Title = p.Title;    // Reset title
                }
                if (project.Title == null) project.Title = "";
                DBProject.Update(project.Id, project.Title, project.Status, string.Join(',', project.Tags));
                return RedirectToAction("Change", new { id = project.Id });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTextBox(TextBoxReference tbReference)
        {
            if (tbReference?.TextBox?.Title == null) tbReference.TextBox.Title = "";
            if (ModelState.IsValid)
            {
                TextBox tb = tbReference.TextBox;
                if (tb.Title == "delete")
                {
                    DBTextBox.Delete(tb.Id);
                    return RedirectToAction("Change", new { id = tbReference.ProjectId });
                }
                DBTextBox.Update(tb.Id, tb.Title, tb.Type);

                for (int i = 0; i < tbReference.TextBox.Elements?.Count; i++)
                {
                    TextBoxElement element = tbReference.TextBox.Elements[i];
                    if (element.Content == null) DBTextBoxElement.Delete(element.Id);
                    else DBTextBoxElement.Update(element.Id, element.Content, element.Type);
                }

                return RedirectToAction("Change", new { id = tbReference.ProjectId, n = tbReference.TextBoxIndex });

            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Add 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTextBox(Project project)
        {
            DBTextBox.Create(project.Id, "", 0);
            return RedirectToAction("Change", new { id = project.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddElement(TextBoxReference tbReference)
        {
            DBTextBoxElement.Create(tbReference.TextBox.Id, "", 0);
            return RedirectToAction("Change", new { id = tbReference.ProjectId, n = tbReference.TextBoxIndex });
        }
        #endregion


        public IActionResult UploadImage(string fileName)
        {
            ViewBag.FileName = fileName;
            return View("UploadImage");
        }

        [HttpPost]
        public IActionResult PostUploadImage(ImageReference imageReference)
        {
            IFormFile postedFile = imageReference.IFormFile;

            string path = Path.Combine(Environment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //string fileName = Path.GetFileName(postedFile.FileName);
            string fileName = GenerateFileName(postedFile) + Path.GetExtension(postedFile.FileName);
            string fullPath = Path.Combine(path, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                postedFile.CopyTo(stream);

            }

            return RedirectToAction("UploadImage", new { fileName = fileName });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProject(Project project)
        {
            if (ModelState.IsValid)
            {
                DBProject.Create(project.Title, project.Status, project.Tags);
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Projects()
        {
            var mProjects = DBProject.Load();

            List<Project> projects = new List<Project>();

            foreach (var row in mProjects)
            {
                projects.Add(new Project
                {
                    Id = row.Id,
                    Title = row.Title,
                    Status = row.Status,
                    Tags = row.Tags.Split(",")
                });
            }

            return View(projects);
        }





        Project GetProject(ColorsData.Models.MProject mProject)
        {
            // Set Project
            Project project = new Project
            {
                Id = mProject.Id,
                Title = mProject.Title,
                Status = mProject.Status,
                Tags = mProject.Tags.Split(",")
            };


            // Get TextBoxes
            var mTextBoxes = DBTextBox.Load(mProject.Id);
            List<TextBox> textBoxes = new List<TextBox>();
            foreach (var mTextBox in mTextBoxes)
            {
                textBoxes.Add(GetTextBox(mTextBox));
            }

            project.TextBoxes = textBoxes;

            // Return
            return project;
        }


        TextBox GetTextBox(ColorsData.Models.MTextBox mTextBox)
        {
            // Set TextBox
            TextBox box = new TextBox
            {
                Id = mTextBox.Id,
                Title = mTextBox.Title,
                Type = mTextBox.Type
            };

            // Set Elements
            var mElements = DBTextBoxElement.Load(mTextBox.Id);
            List<TextBoxElement> elements = new List<TextBoxElement>();
            foreach (var mElement in mElements)
            {
                elements.Add(GetTextBoxElement(mElement));
            }
            box.Elements = elements;
            return box;
        }

        TextBoxElement GetTextBoxElement(ColorsData.Models.MTextBoxElement mElement)
        {
            return new TextBoxElement
            {
                Id = mElement.Id,
                Content = mElement.Content,
                Type = mElement.Type
            };
        }

        string GenerateFileName(IFormFile file)
        {
            using SHA256 sha = SHA256.Create();
            
            Stream fileStream = file.OpenReadStream();

            byte[] bytes = sha.ComputeHash(fileStream);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
