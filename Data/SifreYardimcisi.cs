using System.Security.Cryptography;
using System.Text;

namespace KutuphaneOtomasyon.Data
{
    public static class SifreYardimcisi
    {
        private const string Salt = "KutuphaneOtomasyon_Salt_2026";

        public static string Hashle(string duzMetinSifre)
        {
            using var sha256 = SHA256.Create();
            var girdi = Encoding.UTF8.GetBytes(duzMetinSifre + Salt);
            var hashBaytlari = sha256.ComputeHash(girdi);
            return Convert.ToBase64String(hashBaytlari);
        }

        public static bool Dogrula(string duzMetinSifre, string hash)
        {
            return Hashle(duzMetinSifre) == hash;
        }
    }
}
