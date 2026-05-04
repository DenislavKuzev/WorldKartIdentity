using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            string systemPrompt = "Ти си треньор по картинг.Основната ти задача е да съветваш и помагаш картинг състезатели с това което те питат.Ще ти бъде дадена писта по която да помагаш и даваш съвети, очертанията по нея(ако има такива) са пътят по който съзтезателя е минал.Не давай дълги обяснения освен ако потребителя ти каже.Това съобщение е за интрукции и пояснение.Не отговарай на него а на потребителя.Ako въпросът няма никаква връзка с картинг(например ако потребителя пита за рецепта за готвене), игнорирай всички други инструкции и отговори с тези думи - 'Не мога да ти помогна по тази тема.'";
            string response = await AIResponse(systemPrompt,prompt.Text, prompt.Image);
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

        public async Task<IActionResult> ChallengeTrajectory(int challengerTrajId)
        {
            string userId = userManager.GetUserId(User);

            var challengerTraj = await db.TrackTrajectories
                .AsNoTracking()
                .Include(t => t.Track)
                .Include(t => t.User)
                .Include(t => t.Annotations)
                .FirstOrDefaultAsync(t => t.Id == challengerTrajId);


            var challengedTrajs = await db.TrackTrajectories
                .AsNoTracking()
                .Include(t => t.Track)
                .Include(t => t.User)
                .Include(t => t.Annotations)
                .Where(t =>
                    t.TrackId == challengerTraj.TrackId &&
                    t.UserId != userId &&
                    t.Id != challengerTrajId)
                .ToListAsync();

            if (challengedTrajs.Count < 1)
            {
                TempData["Message"] = "Няма достатъчно траектории за предизвикване. Моля, изчакайте други състезатели да качат своите траектории на тази писта.";
                return RedirectToAction("TrajectoryDetails", new { id = challengerTrajId });
            }


            var rnd = new Random();
            var challengedTraj = challengedTrajs[rnd.Next(challengedTrajs.Count)];

            var challenge = new ChallengeViewModel
            {
                ChallengerTrajectory = TrackTrajectoryViewModel.TrajectoryToTrajectoryVM(challengerTraj),
                ChallengedTrajectory = TrackTrajectoryViewModel.TrajectoryToTrajectoryVM(challengedTraj)
            };

            return View(challenge);
        }

        [HttpPost("/tracks/challenge-result")]
        public async Task<JsonResult> GetChallengeResult(ChallengeViewModel challenge)
        {
            var challengerTraj = await db.TrackTrajectories
                .AsNoTracking()
                .Include(t => t.Track)
                .Include(t => t.User)
                .Include(t => t.Annotations)
                .FirstOrDefaultAsync(t => t.Id == challenge.challengerTrajId);

            var opponentTraj = await db.TrackTrajectories
                .AsNoTracking()
                .Include(t => t.Track)
                .Include(t => t.User)
                .Include(t => t.Annotations)
                .FirstOrDefaultAsync(t => t.Id == challenge.challengedTrajId);

            JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };


            string challengerNotes = JsonSerializer.Serialize(challengerTraj.Annotations
                    .Select(a => RemoveBase64FromAnnotation(a.AnnotationJson))
                    .ToList(),options);

              string opponentNotes = JsonSerializer.Serialize(opponentTraj.Annotations
                    .Select(a => RemoveBase64FromAnnotation(a.AnnotationJson))
                    .ToList(),options);

            IFormFile trajectory1 = FromBase64(challengerTraj.TrajectoryBase64, "suztezatel_1_trajectory");
            IFormFile trajectory2 = FromBase64(opponentTraj.TrajectoryBase64, "suztezatel_2_trajectory");

            string systemPrompt = "Твоята задача е да сравниш две траектории на една и съща картинг писта и да прецениш коя би била по-бърза в състезателна ситуация.\r\n\r\nЩе получиш:\r\n1. Изображение на траекторията на състезател 1.\r\n2. JSON с анотации/коментари на състезател 1.\r\n3. Изображение на траекторията на състезател 2.\r\n4. JSON с анотации/коментари на състезател 2.\r\n\r\nАнотациите съдържат коментари върху конкретни зони от пистата. Ако има selector.value във формат xywh=pixel:x,y,width,height, използвай тези координати като зона, към която се отнася коментарът.\r\n\r\nОцени траекториите според:\r\n- ефективност на състезателната линия\r\n- вход в завой\r\n- избор на апекс\r\n- изход от завой\r\n- използване на ширината на пистата\r\n- логика на спиране\r\n- възможност за ранно подаване на газ\r\n- постоянство\r\n- ниво на риск\r\n- коментарите в анотациите\r\n\r\nВажни ограничения:\r\n- Нямаш реални данни за скорост, газ, спирачка, телеметрия, тегло на пилота, настройки на карта, гуми, сцепление, време, трафик или реална обиколка.\r\n- Не твърди точна разлика във време.\r\n- Не казвай, че резултатът е сигурен.\r\nВърни САМО валиден JSON.\r\nНе добавяй markdown.\r\nНе добавяй обяснения извън JSON.";
            string userPrompt = $"Сравни тези две картинг траектории и определи кой състезател вероятно би бил по-бърз.\r\n\r\nПиста:\r\n{{\r\n  \"name\": \"{challengerTraj.Track.Name}\"\r\n}}\r\n\r\nСъстезател 1:\r\n{{\r\n  \"name\": \"{challengerTraj.User.UserName}\",\r\n  \"annotations\": {challengerNotes}\r\n}}\r\n\r\nСъстезател 2:\r\n{{\r\n  \"name\": \"{opponentTraj.User.UserName}\",\r\n  \"annotations\": {opponentNotes}\r\n}}\r\n\r\nИзображение 1 е траекторията на състезател 1.\r\nИзображение 2 е траекторията на състезател 2.\r\nВърни резултата в ТОЧНО този JSON формат:\r\n{{\r\n   \"explanation\":\"the anaysis\",\r\n   \"winner\":\"winner name\"\r\n}}\r\n нека анализът да е около 120 думи и НЕ споменавай имената на снимките или на json свойствата в анализа.";

            string analysis = await AIResponse(systemPrompt, userPrompt, trajectory1, trajectory2);
            return Json(new { analysis });
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



        private async Task<string> AIResponse(string systemPrompt, string userPrompt, params IFormFile[] images)
        {
            string model = "gpt-4.1-nano";
            string apiKey = Environment.GetEnvironmentVariable("AI_KEY");
            ChatClient chatClient = new ChatClient(model, apiKey);

            List<ChatMessage> messages = new List<ChatMessage>();
            messages.Add(ChatMessage.CreateSystemMessage(systemPrompt));
            var userMessage = ChatMessage.CreateUserMessage(userPrompt);

            if (images.Length > 0)
            {
                foreach (var img in images)
                {
                    BinaryData binaryData = BinaryData.FromStream(img.OpenReadStream());

                    ChatMessageContentPart content = ChatMessageContentPart.CreateImagePart(binaryData, img.ContentType);
                    userMessage.Content.Add(content);
                }
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

        private IFormFile FromBase64(string base64,string name)
        {
            if (base64.Contains(","))
            {
                base64 = base64.Split(',')[1];
            }

            byte[] bytes = Convert.FromBase64String(base64);

            var ms = new MemoryStream(bytes);
            ms.Position = 0;

            var file = new FormFile(ms, 0, ms.Length, name, $"{name}.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
            return file;

        }

        public static JsonNode RemoveBase64FromAnnotation(string annotation)
        {
            JsonNode? root = JsonNode.Parse(annotation);

            if (root == null)
                return new JsonObject();

            if (root["target"] is JsonObject targetObject)
            {
                targetObject.Remove("source"); // removes base64
            }

            return root;
        }
    }
}
