using aurga.Model;

namespace aurga.Common
{
    public static class SharedStore
    {
        public static List<UserInfo> Users { get; set; } = new List<UserInfo>();
        static SharedStore() { }
    }
}
