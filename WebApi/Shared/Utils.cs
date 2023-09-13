using System.Text;
using System.Text.RegularExpressions;

namespace WebApi.Shared
{
    public class Utils
    {
        private Utils()
        {            
        }
        static readonly Utils _instance=new Utils();
        public static Utils GetInstance()
        {
            return _instance;
        }

        public string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }  
    }
}