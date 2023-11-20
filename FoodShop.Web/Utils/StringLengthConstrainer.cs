using System.Security.Policy;

namespace FoodShop.Web.Utils
{
    public static class StringLengthConstrainer
    {
        public static string Constraint(string s, int maxLength, string stub = "...")
        {
            if (s.Length <= maxLength)
            {
                return s;
            }
            return s.Substring(0, maxLength - stub.Length) + stub;
        }
    }
}
