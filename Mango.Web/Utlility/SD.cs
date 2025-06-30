namespace Mango.Web.Utlility
{
    public class SD
    {
        public static string CouponApiBase { get; set; }
        public static string AuthApiBase { get; set; }
        public static string ProductApiBase { get; set; }
        public static string ShopingCartApiBase { get; set; }





        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JWTToken";

       
      public enum ApiType
        {
            Get,
            Post,
            Put,
            Delete
        }
    }
}
