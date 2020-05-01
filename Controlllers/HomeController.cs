using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {

                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Somebody is alredy using this Email");
                    return View("Index");
                }
                
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);

                dbContext.Add(user);
                dbContext.SaveChanges();
                User userindb = dbContext.Users
                    .FirstOrDefault(u => u.Email == user.Email);
                HttpContext.Session.SetInt32("UserId", userindb.UserId);
                return RedirectToAction("dashboard");
            }
            return View("Index");
        }

        [HttpGet("Denys_rufflesnups")]
        public ViewResult LoginUser()
        {
            return View();
        }


        [HttpPost("login")]
        public IActionResult Login(LoginUser submission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == submission.LoginEmail);

                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("LoginUser");
                }

                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(submission, userInDb.Password, submission.LoginPassword);

                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                    return View("LoginUser");
                }
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("LoginUser");
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginUser");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? IdFromSession = HttpContext.Session.GetInt32("UserId");
                if (IdFromSession == null)
                {
                    return RedirectToAction("LoginUser");
                }
            WeddingUserW vMod = new WeddingUserW();
            vMod.Allweddings = dbContext.Weddings
                .Where( w => w.Date > DateTime.Now)
                .Include(w => w.Guests)
                .ThenInclude(g => g.User).ToList();
                
            User ourdude = dbContext.Users
                .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            vMod.user = ourdude;
            
            return View(vMod);
        }

        [HttpGet("NewWedding")]

        public IActionResult NewWedding()
        {
            return View();
        }

        [HttpPost("AddWedding")]
        public IActionResult AddWedding(WeddingUserW fromForm)
        {
            if (ModelState.IsValid)
                {
                    int? IdFromSession = HttpContext.Session.GetInt32("UserId");
                    if (IdFromSession == null)
                    {
                        return RedirectToAction("LoginUser");
                    }
                    if (fromForm.wedding.Date < DateTime.Now)
                    {
                        ModelState.AddModelError("Date", "Date must be in the future");
                        return RedirectToAction("NewWedding");
                    }
                    Wedding ourwed = new Wedding();
                    User redshirt = dbContext.Users
                        .FirstOrDefault(u => u.UserId == IdFromSession);
                    // fromForm.wedding.Creator = redshirt;
                    fromForm.wedding.UserId = (int)IdFromSession;
                    dbContext.Weddings.Add(fromForm.wedding);
                    dbContext.SaveChanges();
                    return RedirectToAction("ViewWedding", new{WeddingId = fromForm.wedding.WeddingId});
                }
                else
                {
                    return View("NewWedding");
                }
                
        }

        [HttpGet("ViewWedding/{WeddingId}")]
        public IActionResult ViewWedding(int WeddingId)
        {
            Wedding vMod = dbContext.Weddings
                .Include(w => w.Guests)
                .ThenInclude(g => g.User).FirstOrDefault(w => w.WeddingId == WeddingId);
            return View(vMod);
        }

        [HttpGet("RSVP/{weddingId}")]

        public IActionResult RSVP(int weddingId)
        {
            Guest NewGuest = new Guest();
            NewGuest.WeddingId = weddingId;
            NewGuest.UserId= (int)HttpContext.Session.GetInt32("UserId");
            dbContext.Guests.Add(NewGuest);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("UNRSVP/{weddingId}")]
        public IActionResult UNRSVP(int weddingId)
        {
                Guest ourguest = dbContext.Guests.FirstOrDefault(g => g.WeddingId == weddingId && g.UserId == (int)HttpContext.Session.GetInt32("UserId"));
                dbContext.Guests.Remove(ourguest);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
        }

        [HttpGet("Delete/{weddingId}")]
        public IActionResult Delete(int weddingId)
        {
            Wedding wedding = dbContext.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);
            dbContext.Weddings.Remove(wedding);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}