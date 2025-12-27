using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldKartIdentity.Database;

public class TrackLikeController : Controller
{
    private readonly ApplicationDbContext db;
    private readonly UserManager<User> userManager;

    public TrackLikeController(ApplicationDbContext db, UserManager<User> userManager)
    {
        this.db = db;
        this.userManager = userManager;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Like(int trackId)
    {
        var userId = userManager.GetUserId(User);

        var exists = await db.TrackLikes
            .AnyAsync(x => x.UserId == userId && x.TrackId == trackId);

        if (!exists)
        {
            db.TrackLikes.Add(new TrackLike
            {
                UserId = userId,
                TrackId = trackId
            });

            await db.SaveChangesAsync();
        }

        return RedirectToAction("TrackGallery");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Unlike(int trackId)
    {
        var userId = userManager.GetUserId(User);

        var like = await db.TrackLikes
            .FirstOrDefaultAsync(x => x.UserId == userId && x.TrackId == trackId);

        if (like != null)
        {
            db.TrackLikes.Remove(like);
            await db.SaveChangesAsync();
        }

        return RedirectToAction("TrackGallery");
    }




    //[Authorize]
    //[HttpPost]
    //public async Task<IActionResult> Toggle(int trackId, string returnUrl)
    //{
    //    var userId = userManager.GetUserId(User);

    //    var like = await db.TrackLikes
    //        .FirstOrDefaultAsync(x => x.UserId == userId && x.TrackId == trackId);

    //    if (like == null)
    //    {
    //        db.TrackLikes.Add(new TrackLike
    //        {
    //            UserId = userId,
    //            TrackId = trackId
    //        });
    //    }
    //    else
    //    {
    //        db.TrackLikes.Remove(like);
    //    }

    //    await db.SaveChangesAsync();

    //    return LocalRedirect(returnUrl ?? "/");
    //}

}
