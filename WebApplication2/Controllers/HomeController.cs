using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _usersRoot = "Users";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HomeViewModel hvm = new HomeViewModel();
            hvm.Photos = new List<string>();

            string rootDir = "wwwroot";
            var folderPath = Path.Combine(rootDir, "Uploads");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            foreach (var myphoto in Directory.GetFiles(folderPath))
            {
                hvm.Photos.Add(Path.GetFileName(myphoto));
            }

            return View(hvm);
        }

        [HttpPost]
        public IActionResult Index(List<IFormFile> FileUploads)
        {
            string rootDir = "wwwroot";
            var folderPath = Path.Combine(rootDir, "Uploads");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (FileUploads != null && FileUploads.Count > 0)
            {
                foreach (var myfile in FileUploads)
                {
                    var filePath = Path.Combine(folderPath, DateTime.Now.Ticks + Path.GetExtension(myfile.FileName));

                    using (var mystream = System.IO.File.Create(filePath))
                    {
                        myfile.CopyTo(mystream);
                    }
                }
            }

            HomeViewModel hvm = new HomeViewModel();
            hvm.Photos = new List<string>();
            foreach (var myphoto in Directory.GetFiles(folderPath))
            {
                hvm.Photos.Add(Path.GetFileName(myphoto));
            }

            return View(hvm);
        }

        public IActionResult DeletePhoto(string PhotoName)
        {
            string rootDir = "wwwroot";
            var folderPath = Path.Combine(rootDir, "Uploads");

            if (System.IO.File.Exists(Path.Combine(folderPath, PhotoName)))
            {
                System.IO.File.Delete(Path.Combine(folderPath, PhotoName));
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SaveComment(string Comment)
        {
            if (!Directory.Exists("Comments"))
            {
                Directory.CreateDirectory("Comments");
            }
            System.IO.File.WriteAllText(Path.Combine("Comments", "entry_" + DateTime.Now.Ticks + ".txt"), Comment);
            var resp = new CommentsViewModel();
            resp.CommentSaved = true;
            resp.StatusMessage = "Comment saved successfully!";
            return View("Comments", resp);
        }

        public IActionResult DeleteComment(string TargeFile)
        {
            if (System.IO.File.Exists(Path.Combine("Comments", TargeFile)))
            {
                System.IO.File.Delete(Path.Combine("Comments", TargeFile));
            }
            CommentsViewModel resp = new CommentsViewModel();
            resp.CommentSaved = true;
            resp.StatusMessage = "Comment deleted successfully!";
            return RedirectToAction("Comments", "Home");
        }

        public IActionResult Comments()
        {
            if (!Directory.Exists("Comments"))
            {
                Directory.CreateDirectory("Comments");
            }
            return View(new CommentsViewModel());
        }

        public IActionResult Users()
        {
            var users = LoadAllUsers();
            return View(users);
        }


        public IActionResult FileManage(string Name, string Gender, string Nationality, string Religion, string About_me, string Lifestyle, string Birthday, string Address, string Age, string Contact)
        {
            // Save each field into its folder (using the file name " .txt" as in your code)
            System.IO.File.WriteAllText("Name/ .txt", Name);
            System.IO.File.WriteAllText("Age/ .txt", Age);
            System.IO.File.WriteAllText("Gender/ .txt", Gender);
            System.IO.File.WriteAllText("Birthday/ .txt", Birthday);
            System.IO.File.WriteAllText("Nationality/ .txt", Nationality);
            System.IO.File.WriteAllText("Religion/ .txt", Religion);
            System.IO.File.WriteAllText("Contact/ .txt", Contact);
            System.IO.File.WriteAllText("Address/ .txt", Address);
            System.IO.File.WriteAllText("Aboutme/ .txt", About_me);
            System.IO.File.WriteAllText("Lifestyle/ .txt", Lifestyle);

            // After saving, redirect to the Profile display page.
            return RedirectToAction("Privacy", "Home");
        }

        [HttpPost]
       
            [HttpPost]
            public IActionResult SaveUser(UserProfile user)
            {
                SaveUserToFiles(user);
                return RedirectToAction("Users");
        }


        [HttpPost]
        public IActionResult DeleteUser(string username)
        {
            DeleteUserDirectory(username);
            return RedirectToAction("Users");
        }

        private List<UserProfile> LoadAllUsers()
        {
            var users = new List<UserProfile>();

            Directory.CreateDirectory(_usersRoot);

            foreach (var userDir in Directory.GetDirectories(_usersRoot))
            {
                users.Add(new UserProfile
                {
                    Username = Path.GetFileName(userDir),
                    Name = ReadUserFile(userDir, "Name"),
                    Age = ReadUserFile(userDir, "Age"),
                    Gender = ReadUserFile(userDir, "Gender"),
                    Birthday = ReadUserFile(userDir, "Birthday"),
                    Nationality = ReadUserFile(userDir, "Nationality"),
                    Religion = ReadUserFile(userDir, "Religion"),
                    Contact = ReadUserFile(userDir, "Contact"),
                    Address = ReadUserFile(userDir, "Address"),
                    About_me = ReadUserFile(userDir, "About_me"),
                    Lifestyle = ReadUserFile(userDir, "Lifestyle"),
                    ProfileImage = ReadUserFile(userDir, "ProfileImage")
                });
            }

            return users;
        }

        private void SaveUserToFiles(UserProfile user)
        {
            var userDir = Path.Combine(_usersRoot, user.Username);
            Directory.CreateDirectory(userDir);

            SaveUserFile(userDir, "Name", user.Name);
            SaveUserFile(userDir, "Age", user.Age);
            SaveUserFile(userDir, "Gender", user.Gender);
            SaveUserFile(userDir, "Birthday", user.Birthday);
            SaveUserFile(userDir, "Nationality", user.Nationality);
            SaveUserFile(userDir, "Religion", user.Religion);
            SaveUserFile(userDir, "Contact", user.Contact);
            SaveUserFile(userDir, "Address", user.Address);
            SaveUserFile(userDir, "About_me", user.About_me);
            SaveUserFile(userDir, "Lifestyle", user.Lifestyle);
            SaveUserFile(userDir, "ProfileImage", user.ProfileImage);
        }

        private void DeleteUserDirectory(string username)
        {
            var userDir = Path.Combine(_usersRoot, username);
            if (Directory.Exists(userDir))
            {
                Directory.Delete(userDir, true);
            }
        }

        private string ReadUserFile(string userDir, string field)
        {
            var path = Path.Combine(userDir, $"{field}.txt");
            return System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : "";
        }

        private void SaveUserFile(string userDir, string field, string value)
        {
            var path = Path.Combine(userDir, $"{field}.txt");
            System.IO.File.WriteAllText(path, value);
        }

        public IActionResult Profile(string username)
        {
            var model = new UserProfile();

            if (string.IsNullOrEmpty(username))
            {
                // Handle missing username
                return RedirectToAction("Users");
            }

            var usersDir = Path.Combine(Directory.GetCurrentDirectory(), "Users");
            var userDir = Path.Combine(usersDir, username);

            if (Directory.Exists(userDir))
            {
                model.Username = username;
                model.Name = ReadFile(userDir, "Name.txt");
                model.Age = ReadFile(userDir, "Age.txt");
                model.Gender = ReadFile(userDir, "Gender.txt");
                model.Birthday = ReadFile(userDir, "Birthday.txt");
                model.Nationality = ReadFile(userDir, "Nationality.txt");
                model.Religion = ReadFile(userDir, "Religion.txt");
                model.Contact = ReadFile(userDir, "Contact.txt");
                model.Address = ReadFile(userDir, "Address.txt");
                model.About_me = ReadFile(userDir, "About_me.txt");
                model.Lifestyle = ReadFile(userDir, "Lifestyle.txt");
                model.ProfileImage = ReadFile(userDir, "ProfileImage.txt");
            }

            return View(model);
        }



        private string ReadFile(string directory, string filename)
        {
            var path = Path.Combine(directory, filename);
            return System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : "";
        }

        [HttpPost]
        public IActionResult UpdateProfile(UserProfile user, IFormFile ProfileImage)
        {
            var usersDir = Path.Combine(Directory.GetCurrentDirectory(), "Users");
            var originalUsername = Request.Form["OriginalUsername"];

            if (!string.IsNullOrEmpty(originalUsername) && originalUsername != user.Username)
            {
                var oldDir = Path.Combine(usersDir, originalUsername);
                var newDir = Path.Combine(usersDir, user.Username);

                if (Directory.Exists(oldDir) && !Directory.Exists(newDir))
                {
                    Directory.Move(oldDir, newDir);
                }
            }

            if (ProfileImage != null)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                var filePathImage = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePathImage, FileMode.Create))
                {
                    ProfileImage.CopyTo(stream);
                }

                user.ProfileImage = fileName;
            }

            var userDir = Path.Combine(usersDir, user.Username);
            Directory.CreateDirectory(userDir);

            System.IO.File.WriteAllText(Path.Combine(userDir, "Name.txt"), user.Name);
            System.IO.File.WriteAllText(Path.Combine(userDir, "Age.txt"), user.Age);
            System.IO.File.WriteAllText(Path.Combine(userDir, "Gender.txt"), user.Gender);
            System.IO.File.WriteAllText(Path.Combine(userDir, "Birthday.txt"), user.Birthday);
            System.IO.File.WriteAllText(Path.Combine(userDir, "Nationality.txt"), user.Nationality);
            System.IO.File.WriteAllText(Path.Combine(userDir, "Religion.txt"), user.Religion);
            System.IO.File.WriteAllText(Path.Combine(userDir, "Contact.txt"), user.Contact);
            System.IO.File.WriteAllText(Path.Combine(userDir, "Address.txt"), user.Address);
            System.IO.File.WriteAllText(Path.Combine(userDir, "About_me.txt"), user.About_me);
            System.IO.File.WriteAllText(Path.Combine(userDir, "Lifestyle.txt"), user.Lifestyle);
            System.IO.File.WriteAllText(Path.Combine(userDir, "ProfileImage.txt"), user.ProfileImage);

            return RedirectToAction("Profile", new { username = user.Username });
        }


        [HttpPost]
        public IActionResult UploadPhoto(List<IFormFile> FileUploads)
        {
            return RedirectToAction("Profile");
        }
    }
}