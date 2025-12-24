using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Admin()
        {
            ViewBag.KartingTracks = 0;
            ViewBag.Trajectories = 0;
            ViewBag.Users = 0;

            return View();
        }

        public IActionResult TrackGallery(int id)
        {
            var tracks = db.Tracks.ToList();
            var tracksVM = new List<TrackViewModel>();
            foreach (var track in tracks)
            {
                var trackVM = TrackViewModel.TrackToTrackVM(track);
                tracksVM.Add(trackVM);
            }
            return View(tracksVM);
        }

        public async Task<IActionResult> TrackDetail(int id)
        {
            var track = await db.Tracks.FindAsync(id);
            TrackViewModel viewModel = new TrackViewModel(track);
            return View(viewModel);
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
            if (trackVM.PictureFile != null && trackVM.PictureFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    trackVM.PictureFile.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    string base64String = Convert.ToBase64String(imageBytes);
                    trackVM.PictureBase64 = base64String;
                }
            }   //Trqbwa da si suzdam PictureFile vuv TrackViewModel
            Track tracks = TrackViewModel.TrackVMToTrack(trackVM);
            db.Tracks.Add(tracks);
            db.SaveChanges();
            return RedirectToAction("TrackGallery");
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
            db.TrackRequests.Add(trackrequest);
            db.SaveChanges();
            return View();
        }
    }
}
