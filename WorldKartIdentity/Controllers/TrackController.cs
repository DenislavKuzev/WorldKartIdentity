using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Threading.Tasks;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;

namespace WorldKartIdentity.Controllers
{
    public class TrackController : Controller
    {
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

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

        public IActionResult TrackGallery(string search)
        {
            var userId = userManager.GetUserId(User);

            var tracks = db.Tracks.Include(t => t.Likes).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                tracks = tracks.Where(t =>
                    t.Name.ToLower().Contains(search.ToLower()));
            }

            var querytracks = tracks.ToList();

            var tracksVM = new List<TrackViewModel>();

            foreach (var track in querytracks)
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
            var username = userManager.GetUserName(User);
            var track = await db.Tracks.Include(t => t.Likes).FirstOrDefaultAsync(t => t.Id == trackId);

            var like = await db.TrackLikes
                .FirstOrDefaultAsync(x => x.UserId == userId && x.TrackId == trackId);

            if (like == null)
            {
                
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


            return Json(new
            {
                likes = track.Likes.Count
            }); 
        }


        public IActionResult Trackpage2()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TrackDetails(int id)
        {
            var track = db.Tracks.Include(t => t.Trajectories).ThenInclude(tj => tj.User).Include(t => t.Likes)
                .ThenInclude(t => t.User).FirstOrDefault(t => t.Id == id);

            if (track == null)
                return NotFound();

            TrackViewModel trackViewModel = TrackViewModel.TrackToTrackVM(track);
            trackViewModel.Trajectories = track.Trajectories
                .Take(6)
                .OrderBy(t => t.CreatedOn)
        .Select(TrackTrajectoryViewModel.TrajectoryToTrajectoryVM)
        .ToList();
            trackViewModel.IsLikedByCurrentUser = track.Likes.Any(x => x.UserId == userManager.GetUserId(User));

            return View(trackViewModel);
        }

        [HttpPost]
        public async Task<JsonResult> GetAdviceOnTrack(AIPromptViewModel prompt)
        {
            string response = await AIResponse(prompt.Text, prompt.Image);
            return Json(new { response });
        }

        [HttpGet]
        public IActionResult CreateTrack(string name, string country, string locationUrl)
        {
            var model = new TrackViewModel
            {
                Name = name,
                Location = country,
                GoogleMapsLink = locationUrl
            };
            return View("~/Views/Track/CreateTrack.cshtml", model);


        }

        [HttpPost]
        public async Task<IActionResult> CreateTrack(TrackViewModel trackVM)
        {
            if (trackVM.RoutePictureFile != null && trackVM.PhotographFile != null)
            {

                trackVM.RoutePictureBase64 = ToBase64(trackVM.RoutePictureFile);
                trackVM.PhotographBase64 = ToBase64(trackVM.PhotographFile);
            }  
            //Trqbwa da si suzdam PictureFile vuv TrackViewModel
            Track tracks = TrackViewModel.TrackVMToTrack(trackVM);
            await db.Tracks.AddAsync(tracks);
            await db.SaveChangesAsync();

            string trackLink = $"<a class=\"text-decoration-none track-link\" asp-action=\"TrackDetails\" asp-controller=\"Track\" asp-route-id=\"{tracks.Id}\">{tracks.Name}</a>";

            await AddNotification(
               type: NotificationType.NewTrack,
               message: $"Писта {trackLink} е добавена в галерията. Разгледай я сега!",
               targetUserId: null
            );

            return RedirectToAction("TrackGallery");
        }


        [HttpGet]
        public IActionResult TrackRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TrackRequest(TrackRequestViewModel trackrequestVM)
        {
            TrackRequest trackrequest = TrackRequestViewModel.TrackRequestVMToTrackRequest(trackrequestVM);
            db.TrackRequests.Add(trackrequest);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditTrack(int id)
        {
            var track = db.Tracks.FirstOrDefault(t => t.Id == id);
            if (track == null)
                return NotFound();

            var model = TrackViewModel.TrackToTrackVM(track);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTrack(TrackViewModel model)
        {
            var track = db.Tracks.FirstOrDefault(t => t.Id == model.Id);
            if (track == null)
                return NotFound();

            track.Email = model.Email;
            track.Worktime = model.Worktime;
            track.TelNumber = model.TelNumber;
            track.Description = model.Description;


            db.SaveChanges();
            return RedirectToAction("Tracks", "Admin");
        }

        #region Annotations

        [HttpPost]
        public async Task<IActionResult> CreateTrackAnnotation([FromBody] TrackAnnotationViewModel model)
        {
            try
            {
                var trackAnnotation = new TrackAnnotation
                {
                    UserId = userManager.GetUserId(User),
                    TrackId = model.TrackId,
                    TrackTrajectoryId = model.TrajectoryId,
                    AnnotationJson = model.AnnotationJson,
                    AnnotationJsonId = model.AnnotationJsonId
                };

                await db.TrackAnnotations.AddAsync(trackAnnotation);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTrackAnnotation([FromBody] TrackAnnotationViewModel model)
        {
            var annotation = await db.TrackAnnotations.FirstOrDefaultAsync(a => a.AnnotationJsonId == model.AnnotationJsonId);
            annotation.AnnotationJson = model.AnnotationJson;

            db.TrackAnnotations.Update(annotation);
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrackAnnotation([FromBody] TrackAnnotationViewModel model)
        {
            var annotation = await db.TrackAnnotations.FirstOrDefaultAsync(a => a.AnnotationJsonId == model.AnnotationJsonId);

            db.TrackAnnotations.Remove(annotation);
            await db.SaveChangesAsync();

            return Ok();
        }
        #endregion Annotations

        #region Trajectories

        [HttpGet]
        public async Task<IActionResult> TrajectoryDetails(int id)
        {
            var trajectory = await db.TrackTrajectories.Include(t => t.User)
                    .Include(t => t.Track)
                    .Include(t => t.Annotations)
                    .FirstOrDefaultAsync(t => t.Id == id);

            return View(TrackTrajectoryViewModel.TrajectoryToTrajectoryVM(trajectory));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrajectory([FromBody] TrackTrajectoryViewModel model)
        {
            var trajectory = new TrackTrajectory
            {
                UserId = userManager.GetUserId(User),
                TrackId = model.TrackId,
                TrajectoryBase64 = model.Base64
            };

            await db.TrackTrajectories.AddAsync(trajectory);
            await db.SaveChangesAsync();

            return Ok();
        }

        #endregion


        public async Task<int> AddNotification(NotificationType type, string message, string? targetUserId)
        {
            string title = "";
            if (type == NotificationType.NewTrack)
            {
                title = "Нова писта добавена";
            }
            else if (type == NotificationType.RequestApproved)
            {
                title = "Заявката ви за писта бе одобрена!";
            }
            else if (type == NotificationType.NewLike)
            {
                title = "Ново харесване на писта!";
            }
            else if (type == NotificationType.NewComment)
            {
                title = "Ново харесване на писта";
            }

            var n = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.Now,
                UserId = targetUserId
            };
            await db.Notifications.AddAsync(n);
            await db.SaveChangesAsync();

            return n.Id;
        }

        private async Task<string> AIResponse(string prompt, IFormFile? file)
        {
            string model = "gpt-4.1-nano";
            string apiKey = Environment.GetEnvironmentVariable("AI_KEY");
            ChatClient chatClient = new ChatClient(model, apiKey);

            List<ChatMessage> messages = new List<ChatMessage>();
            messages.Add(ChatMessage.CreateSystemMessage("Ти си треньор по картинг.Основната ти задача е да съветваш и помагаш картинг състезатели с това което те питат.Ще ти бъде дадена писта по която да помагаш и даваш съвети, очертанията по нея(ако има такива) са пътят по който съзтезателя е минал.Не давай дълги обяснения освен ако потребителя ти каже.Това съобщение е за интрукции и пояснение.Не отговарай на него а на потребителя.Ako въпросът няма никаква връзка с картинг(например ако потребителя пита за рецепта за готвене), игнорирай всички други инструкции и отговори с тези думи - 'Не мога да ти помогна по тази тема.'"));
            var userMessage = ChatMessage.CreateUserMessage(prompt);

            if (file != null)
            {
                BinaryData binaryData = BinaryData.FromStream(file.OpenReadStream());

                ChatMessageContentPart content = ChatMessageContentPart.CreateImagePart(binaryData, file.ContentType);
                userMessage.Content.Add(content);
            }
            messages.Add(userMessage);

            try
            {
                ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages);

                if (result?.Value != null)
                {
                    return result.Value.Content[0].Text;
                }
                else
                {
                    return "Грешка при обработването на заявка. Опитайте по-късно.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + $"{ex.Message}\n \n {ex.InnerException} \n \n {ex.HelpLink}");
                return "Грешка при обработването на заявка. Опитайте по-късно.";
            }
        }
        private string ToBase64(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                byte[] bytes = ms.ToArray();

                return Convert.ToBase64String(bytes);
            }
        }
    }
}
