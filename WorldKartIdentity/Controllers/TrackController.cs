using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;

namespace WorldKartIdentity.Controllers
{
    public class TrackController : Controller
    {
        private readonly ApplicationDbContext db;
        //private readonly DbSet<Track> track;

        public TrackController(ApplicationDbContext context/*, DbSet<Track> tracks*/)
        {
            db = context;
            //track = tracks;
        }

        public IActionResult Admin()
        {
            ViewBag.KartingTracks = 0;
            ViewBag.Trajectories = 0;
            ViewBag.Users = 0;

            return View();
        }

        public IActionResult TrackGallery()
        {
            var tracks = db.Tracks.ToList();
            return View(tracks);
        }


        //[HttpPost]
        //public IActionResult ToggleLike(int id)
        //{
        //    var track = db.Tracks.FirstOrDefault(t => t.Id == id);

        //    if (track == null)
        //        return NotFound();

        //    track.IsLiked = !track.IsLiked;
        //    db.SaveChanges();

        //    string referer = Request.Headers["Referer"].ToString();
        //    return Redirect(referer);
        //}



        [HttpGet]
        public IActionResult CreateTrack()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTrack(TrackViewModel trackVM)
        {
            Track track = TrackViewModel.TrackVMToTrack(trackVM);
            //db.Tracks.Add(track);
            db.SaveChanges();
            return RedirectToAction("BookList");
        }

        [HttpGet]
        public IActionResult TrackRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TrackRequests(TrackRequestViewModel trackrequestVM)
        {
            TrackRequest trackrequest = TrackRequestViewModel.TrackRequestVMToTrackRequest(trackrequestVM);
            //db.TrackRequests.Add(trackrequest);
            db.SaveChanges();
            return View();
        }
    }
}
