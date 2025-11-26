using Microsoft.AspNetCore.Mvc;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;

namespace WorldKartIdentity.Controllers
{
    public class TrackController : Controller
    {
        private readonly ApplicationDbContext db;

        public TrackController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Library()
        {
            ViewBag.KartingTracks = 0;
            ViewBag.Trajectories = 0;
            ViewBag.Users = 0;

            return View();
        }

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
        public IActionResult TrackRequests()
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
