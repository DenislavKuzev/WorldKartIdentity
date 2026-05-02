using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class BlockedUserViewModel
    {
        public int Id { get; set; } = 0;

        public string? UserName { get; set; } = null;

        public DateTime BlockedOn { get; set; } = DateTime.Now;


        public BlockedUserViewModel()
        { }

        public BlockedUserViewModel(int Id, string UserName, DateTime BlockedOn)
        {
            this.Id = Id;
            this.UserName = UserName;
            this.BlockedOn = BlockedOn;
        }

        public static BlockedUser BlockedUserVMToBlockedUser(BlockedUserViewModel blockedUserVM)
        {
            BlockedUser blockedUser = new BlockedUser();
            blockedUser.Id = blockedUserVM.Id;
            blockedUser.User.UserName = blockedUserVM.UserName;
            blockedUser.BlockedOn = blockedUserVM.BlockedOn;
            return blockedUser;
        }
    }
}
