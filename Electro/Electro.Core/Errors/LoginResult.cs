namespace Electro.Core.Models.Account
{
    public class LoginResult
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string Roles { get; set; } // قائمة الأدوار

    }
}
