using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;

namespace WorldKartIdentity.Controllers
{
    public class TrackController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public TrackController(ApplicationDbContext context, UserManager<User> userManager)
        {
            db = context;
            this.userManager = userManager;
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
            var userId = userManager.GetUserId(User);

            var tracks = db.Tracks.Include(t => t.Likes).ToList();
            var tracksVM = new List<TrackViewModel>();
            foreach (var track in tracks)
            {
                var trackVM = TrackViewModel.TrackToTrackVM(track);
                trackVM.LikesCount = track.Likes.Count;
                trackVM.IsLikedByCurrentUser =
                    track.Likes.Any(x => x.UserId == userId);
                tracksVM.Add(trackVM);
            }
            return View(tracksVM);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int trackId)
        {
            var userId = userManager.GetUserId(User);

            if (trackId == 0)
                return BadRequest("trackId is 0 — not passed from form");

            var like = await db.TrackLikes
                .FirstOrDefaultAsync(x => x.UserId == userId && x.TrackId == trackId);

            if (like == null)
            {
                var track = await db.Tracks.FindAsync(trackId);
                db.TrackLikes.Add(new TrackLike
                {
                    UserId = userId,
                    TrackId = trackId
                });
            }
            else
            {
                db.TrackLikes.Remove(like);
            }

            await db.SaveChangesAsync();
            return RedirectToAction("TrackGallery");
        }


        public async Task<IActionResult> TrackDetail(int id)
        {
            var track = await db.Tracks.FindAsync(id);
            TrackViewModel viewModel = new TrackViewModel(track);
            
            return View(viewModel);
            
        }


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
