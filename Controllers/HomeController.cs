using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using FavoriteMovies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FavoriteMovies.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

//----------------------------------------------
    public User GetCurrentUser()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        if(userId == null)
        {
            return null;
        }
        return _context.Users
        .First(u=>u.UserId==userId);
    }

//----------------------------------------------
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

//----------------------------------------------
        [HttpPost("login")]
        public IActionResult LoginProcess(LoginUser userSubmission)
            {
                if(ModelState.IsValid)
                {
                    // If inital ModelState is valid, query for a user with provided email
                    var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                    // If no user exists with provided email
                    if(userInDb == null)
                    {
                        // Add an error to ModelState and return to View!
                        ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                        return View("Index");
                    }
                    
                    // check pw for match
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                    
                    // result can be compared to 0 for failure
                    if(result == 0)
                    {
                        // handle failure (this should be similar to how "existing email" is handled)
                        ModelState.AddModelError("LoginPassword", "Password Invalid");
                        return View("Index");
                    }
                        HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                        return RedirectToAction("Dashboard", userSubmission);
                }
                return View("Index");
            }
    //----------------------------------------------
        [HttpPost("register")]
        public IActionResult Register(User UserToRegister)
        {
            //check for duplicate emails
            if(_context.Users.Any(u => u.Email == UserToRegister.Email))
            {
                ModelState.AddModelError("Email", "Please Use different Email");
            }
            //check for validation errors
            if(ModelState.IsValid)
            {
            //Hash PW
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                UserToRegister.Password = Hasher.HashPassword(UserToRegister, UserToRegister.Password);
            //insert user into DB
            _context.Add(UserToRegister);
            _context.SaveChanges();
            
            //put id in session
            HttpContext.Session.SetInt32("UserId", UserToRegister.UserId);
            //redirect to dashboard
        
            return RedirectToAction("Dashboard");
            }
            return View("Index");
        }
    //----------------------------------------------------
        [HttpGet("dashboard")]
        public IActionResult DashBoard()
        {
            var currentUser = GetCurrentUser();


            //get user info from session
            int? userId = HttpContext.Session.GetInt32("UserId");
            //check to ensure if logged in
            if(userId == null)
            {
                return RedirectToAction("Index");
            }

            //pass user into view
            ViewBag.CurrentUser = GetCurrentUser();

            ViewBag.AllMovies = _context
            .Movies
            .Include(movie => movie.PostedBy)
            .Include(movie => movie.Likes)
            .OrderByDescending(movie => movie.Likes.Count);
            return View();

        }
//-------------------------------------------------------------
        [HttpPost("movies/{movieId}/likes")]
        public IActionResult AddLike(int movieId)
        {
            //creating a object without a constructor
            var likeToAdd = new Like {
                UserId = GetCurrentUser().UserId,
                MovieId = movieId
            };

            _context.Add(likeToAdd);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

//-------------------------------------------------------------
        [HttpPost("movies/{movieId}/likes/delete")]
        public IActionResult DeleteLike(int movieId)
        {
            var currentUser = GetCurrentUser();

            //get the like
            var likeToDelete = _context
            .Likes
            .First(like => like.MovieId == movieId && like.UserId == currentUser.UserId);

            _context.Remove(likeToDelete);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }
//-------------------------------------------------------------
        [HttpPost("movies/{movieId}/delete")]
        public IActionResult DeleteMovie(int movieId)
        {
            var movieToDelete = _context.Movies
            .First(m => m.MovieId == movieId);

            _context.Remove(movieToDelete);
            _context.SaveChanges();

            return RedirectToAction("dashboard");
        }
//-------------------------------------------------------------
        [HttpGet("movies/new")]
        public IActionResult NewMoviePage()
        {

        return View();
        }
//-------------------------------------------------------------
        [HttpPost("movies")]
        public IActionResult CreateMovie(Movie movieFromForm)
        {
            if(movieFromForm.ReleaseDate >= DateTime.Now)
            {
                //add error here
                ModelState.AddModelError("ReleaseDate", "Please ensure that the date is in the past");
            }
            if(ModelState.IsValid)
            {
                movieFromForm.UserId = (int)HttpContext.Session.GetInt32("UserId");

                _context.Add(movieFromForm);
                _context.SaveChanges();
                return Redirect($"movies/{movieFromForm.MovieId}");
            }
            return View("NewMoviePage");
        }
//-------------------------------------------------------------
        [HttpGet("movies/{movieId}")]
        public IActionResult SingleMoviePage(int movieId)
        {
            //check to ensure theres a current user
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Index");
            }

            //send movie data to the view
            ViewBag.Movie = _context
                .Movies
                .Include(m => m.PostedBy) //gives us access to user who posted
                .Include(m => m.Likes) //pull all the likes
                    .ThenInclude(like => like.UserWhoLikes) //within eachlike give me back user
                .First(m => m.MovieId == movieId);

            ViewBag.CurrentUser = GetCurrentUser();

            return View();
        }
//-------------------------------------------------------------
//-------------------------------------------------------------
//-------------------------------------------------------------
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
