using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Net;

namespace phantom
{
    public class StringHelper
    {
        #region RemoveVietnameseSigns
        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
        #endregion
        public static string ConvertDataTableToString(DataTable aDataTable)
        {
            try
            {
                string[] jSonArray = new string[aDataTable.Rows.Count];
                for (int r = 0; r < aDataTable.Rows.Count; r++)
                {
                    DataRow mRow = aDataTable.Rows[r];
                    string[] mRowArray = new string[aDataTable.Columns.Count];
                    for (int i = 0; i < aDataTable.Columns.Count; i++)
                    {
                        mRowArray[i] = string.Format("\"{0}\" : \"{1}\"", aDataTable.Columns[i].ColumnName, mRow[i].ToString().Replace("\n", "<br/>").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\\", "&#92;").Replace("\"", "&quot;").Replace("'", "&apos;"));
                    }
                    jSonArray[r] = string.Format("{{{0}}}", string.Join(",", mRowArray));
                }
                if (jSonArray.Length == 0)
                    return "";
                return string.Format("[{0}]", string.Join(",", jSonArray));
            }
            catch
            {
                throw;
            }
        }
        public static string CreateStringFromDate()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        public static string Reverse(string text)
        {
            if (text == null) return null;
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }
        public static string ChangeIndexByDouble(string text, int Length)
        {
            char[] mArray = text.ToCharArray();
            char c;
            for (int i = 0; i < mArray.Length; i += Length)
            {
                c = mArray[i];
                mArray[i] = mArray[i + 1];
                mArray[i + 1] = c;
            }
            return new String(mArray);
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string StringToHex(string aInputString)
        {
            try
            {
                aInputString = UnicodeToUtf8(aInputString);
                byte[] mByteArray = Encoding.Default.GetBytes(aInputString);
                return BitConverter.ToString(mByteArray).Replace("-", "").ToLower();
            }
            catch
            {
                throw;
            }
        }
        public static string HexToString(string aInputString)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < aInputString.Length; i += 2)
                {
                    string hs = aInputString.Substring(i, 2);
                    sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
                }
                return Utf8ToUnicode(sb.ToString());
            }
            catch
            {
                throw;
            }
        }
        public static string Utf8ToUnicode(string aInputString)
        {
            try
            {
                byte[] utf8Bytes = new byte[aInputString.Length];
                for (int i = 0; i < aInputString.Length; ++i)
                {
                    utf8Bytes[i] = (byte)aInputString[i];
                }
                return Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length);
            }
            catch
            {
                throw;
            }
        }
        public static string UnicodeToUtf8(string aInputString)
        {
            try
            {
                byte[] utf16Bytes = Encoding.Unicode.GetBytes(aInputString);
                byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);
                // Return UTF8 bytes as ANSI string
                return Encoding.Default.GetString(utf8Bytes);
            }
            catch
            {
                throw;
            }
        }
        public static string RemoveSpecialCharacter(string aInputString)
        {
            string regExp = @"[^\w\d]";
            return Regex.Replace(aInputString, regExp, "");
        }
        public static string ConvertToJsonString<T>(List<T> aData)
        {
            List<string> JSonArray = new List<string>();
            foreach (var item in aData)
            {
                JSonArray.Add(ConvertToJsonString<T>(item));
            }
            return string.Format("[{0}]", string.Join(",", JSonArray));
        }
        public static string ConvertToJsonString<T>(object aData)
        {
            var mPropertiesArray = typeof(T).GetProperties();
            List<string> JSonArray = new List<string>();
            foreach (var mProperty in mPropertiesArray)
            {
                var mValue = mProperty.GetValue(aData, null);
                JSonArray.Add(string.Format("\"{0}\" : \"{1}\"", mProperty.Name, mProperty.GetValue(aData, null)));
            }
            return string.Format("{{{0}}}", string.Join(",", JSonArray));
        }
        public static void ReadJsonValue(string aData, ref Object aClass)
        {
            Dictionary<string, string> ArrayData = new Dictionary<string, string>();
            foreach (Match match in Regex.Matches(aData, "\"([^\"]*)\":\"([^\"]*)\""))
            {
                string reg = match.ToString().Replace("\"", "");
                ArrayData.Add(reg.Split(':')[0], reg.Split(':')[1]);
            }
            var mPropertiesArray = aClass.GetType().GetProperties();
            foreach (var mProperty in mPropertiesArray)
            {
                if (mProperty.PropertyType.Namespace == "System")
                {
                    if (ArrayData.ContainsKey(mProperty.Name))
                        mProperty.SetValue(aClass, Convert.ChangeType(ArrayData[mProperty.Name], mProperty.PropertyType), null);
                }
                else
                {
                    var refClass = Activator.CreateInstance(mProperty.PropertyType);
                    ReadJsonValue(aData, ref refClass);
                    mProperty.SetValue(aClass, Convert.ChangeType(refClass, mProperty.PropertyType), null);
                }
            }
        }
        public static T ReadJsonValue<T>(string aData)
        {
            var mTClass = Activator.CreateInstance<T>();
            var mPropertiesArray = typeof(T).GetProperties();
            foreach (var mProperty in mPropertiesArray)
            {
                if (mProperty.PropertyType.Namespace == "System")
                {
                    if (!string.IsNullOrEmpty(Regex.Match(aData, string.Format("\"{0}\":\"(.+?)\"", mProperty.Name)).Groups[1].Value))
                        mProperty.SetValue(mTClass, Convert.ChangeType(Regex.Match(aData, string.Format("\"{0}\":\"(.+?)\"", mProperty.Name)).Groups[1].Value, mProperty.PropertyType), null);
                    else
                        mProperty.SetValue(mTClass, Convert.ChangeType(Regex.Match(aData, string.Format("{0}:'(.+?)'", mProperty.Name)).Groups[1].Value, mProperty.PropertyType), null);
                }
                else
                {
                    var refClass = Activator.CreateInstance(mProperty.PropertyType);
                    ReadJsonValue(aData, ref refClass);
                    mProperty.SetValue(mTClass, Convert.ChangeType(refClass, mProperty.PropertyType), null);
                }
            }
            return mTClass;
        }
        public static string HtmlEncode(string aData)
        {
            return WebUtility.HtmlEncode(aData);
        }
        public static string HtmlDecode(string aData)
        {
            return WebUtility.HtmlDecode(aData);
        }
        public static string GenerateCode(int aID, int aLength, string aCodePrefix)
        {
            return aCodePrefix + aID.ToString().PadLeft((aLength - aCodePrefix.Length), '0');
        }
        public static string VNCurrency(decimal number)
        {
            string s = number.ToString("#");
            string[] so = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = new string[] { "", "nghìn", "triệu", "tỷ" };
            int i, j, donvi, chuc, tram;
            string str = " ";
            bool booAm = false;
            decimal decS = 0;
            //Tung addnew
            try
            {
                decS = Convert.ToDecimal(s.ToString());
            }
            catch
            {
            }
            if (decS < 0)
            {
                decS = -decS;
                s = decS.ToString();
                booAm = true;
            }
            i = s.Length;
            if (i == 0)
                str = so[0] + str;
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0)
                        chuc = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        chuc = -1;
                    i--;
                    if (i > 0)
                        tram = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        tram = -1;
                    i--;
                    if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                        str = hang[j] + str;
                    j++;
                    if (j > 3) j = 1;
                    if ((donvi == 1) && (chuc > 1))
                        str = "một " + str;
                    else
                    {
                        if ((donvi == 5) && (chuc > 0))
                            str = "lăm " + str;
                        else if (donvi > 0)
                            str = so[donvi] + " " + str;
                    }
                    if (chuc < 0)
                        break;
                    else
                    {
                        if ((chuc == 0) && (donvi > 0)) str = "lẻ " + str;
                        if (chuc == 1) str = "mười " + str;
                        if (chuc > 1) str = so[chuc] + " mươi " + str;
                    }
                    if (tram < 0) break;
                    else
                    {
                        if ((tram > 0) || (chuc > 0) || (donvi > 0)) str = so[tram] + " trăm " + str;
                    }
                    str = " " + str;
                }
            }
            if (booAm) str = "Âm " + str;
            return (str + "đồng chẵn").Replace("  ", " ");
        }
        public static string VNCurrency(double number)
        {
            string s = number.ToString("#");
            string[] so = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = new string[] { "", "nghìn", "triệu", "tỷ" };
            int i, j, donvi, chuc, tram;
            string str = " ";
            bool booAm = false;
            double decS = 0;
            //Tung addnew
            try
            {
                decS = Convert.ToDouble(s.ToString());
            }
            catch
            {
            }
            if (decS < 0)
            {
                decS = -decS;
                s = decS.ToString();
                booAm = true;
            }
            i = s.Length;
            if (i == 0)
                str = so[0] + str;
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0)
                        chuc = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        chuc = -1;
                    i--;
                    if (i > 0)
                        tram = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        tram = -1;
                    i--;
                    if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                        str = hang[j] + str;
                    j++;
                    if (j > 3) j = 1;
                    if ((donvi == 1) && (chuc > 1))
                        str = "một " + str;
                    else
                    {
                        if ((donvi == 5) && (chuc > 0))
                            str = "lăm " + str;
                        else if (donvi > 0)
                            str = so[donvi] + " " + str;
                    }
                    if (chuc < 0)
                        break;
                    else
                    {
                        if ((chuc == 0) && (donvi > 0)) str = "lẻ " + str;
                        if (chuc == 1) str = "mười " + str;
                        if (chuc > 1) str = so[chuc] + " mươi " + str;
                    }
                    if (tram < 0) break;
                    else
                    {
                        if ((tram > 0) || (chuc > 0) || (donvi > 0)) str = so[tram] + " trăm " + str;
                    }
                    str = " " + str;
                }
            }
            if (booAm) str = "Âm " + str;
            return (str + "đồng chẵn").Replace("  ", " ");
        }
        public static byte[] ToByteArray(string input)
        {
            return Encoding.Unicode.GetBytes(input);
        }
        public static string FromByteArray(byte[] input)
        {
            return Encoding.Unicode.GetString(input);
        }
    }
    public class EncodeHelper
    {
        #region Md5
        private static byte[] Md5EncryptData(string data)
        {
#pragma warning disable SYSLIB0021
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
#pragma warning restore SYSLIB0021
        }
        public static string Md5Encode(string data)
        {
            //Default: 123456 - e10adc3949ba59abbe56e057f20f883e
            return BitConverter.ToString(Md5EncryptData(data)).Replace("-", "").ToLower();
        }
        #endregion
        #region Quoted Printables
        private static string DecodeQuotedPrintables(string input, string charSet)
        {
            if (string.IsNullOrEmpty(charSet))
            {
                var charSetOccurences = new Regex(@"=\?.*\?Q\?", RegexOptions.IgnoreCase);
                var charSetMatches = charSetOccurences.Matches(input);
                foreach (Match match in charSetMatches)
                {
                    charSet = match.Groups[0].Value.Replace("=?", "").Replace("?Q?", "");
                    input = input.Replace(match.Groups[0].Value, "").Replace("?=", "");
                }
            }

            Encoding enc = new ASCIIEncoding();
            if (!string.IsNullOrEmpty(charSet))
            {
                try
                {
                    enc = Encoding.GetEncoding(charSet);
                }
                catch
                {
                    enc = new ASCIIEncoding();
                }
            }

            //decode iso-8859-[0-9]
            var occurences = new Regex(@"=[0-9A-Z]{2}", RegexOptions.Multiline);
            var matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                try
                {
                    byte[] b = new byte[] { byte.Parse(match.Groups[0].Value.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier) };
                    char[] hexChar = enc.GetChars(b);
                    input = input.Replace(match.Groups[0].Value, hexChar[0].ToString());
                }
                catch
                {; }
            }

            //decode base64String (utf-8?B?)
            occurences = new Regex(@"\?utf-8\?B\?.*\?", RegexOptions.IgnoreCase);
            matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                byte[] b = Convert.FromBase64String(match.Groups[0].Value.Replace("?utf-8?B?", "").Replace("?UTF-8?B?", "").Replace("?", ""));
                string temp = Encoding.UTF8.GetString(b);
                input = input.Replace(match.Groups[0].Value, temp);
            }

            input = input.Replace("=\r\n", "");

            return input;
        }
        private static string DecodeQuotedPrintables(string input)
        {
            var occurences = new Regex(@"=[0-9A-Z]{2}", RegexOptions.Multiline);
            var matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                char hexChar = (char)Convert.ToInt32(match.Groups[0].Value.Substring(1), 16);
                input = input.Replace(match.Groups[0].Value, hexChar.ToString());
            }
            return input.Replace("=\r\n", "");
        }
        private static string DecodeFromUtf8(string utf8String)
        {
            // copy the string as UTF-8 bytes.
            byte[] utf8Bytes = new byte[utf8String.Length];
            for (int i = 0; i < utf8String.Length; ++i)
            {
                //Debug.Assert( 0 <= utf8String[i] && utf8String[i] <= 255, "the char must be in byte's range");
                utf8Bytes[i] = (byte)utf8String[i];
            }
            return Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length);
        }
        public static string ConvertQuotedPrintablesToUnicode(string input)
        {
            return DecodeFromUtf8(DecodeQuotedPrintables(input));
        }
        #endregion
    }
}
public static class StringExtention
{
    public static string GetValueOrDefault(this string aInput, string aDefaultValue = "")
    {
        return aInput == null ? aDefaultValue : aInput;
    }
    public static string RemoveVietnameseSign(this string aInput)
    {
        return phantom.StringHelper.RemoveSign4VietnameseString(aInput);
    }
}