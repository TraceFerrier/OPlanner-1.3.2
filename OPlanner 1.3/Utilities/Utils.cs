using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PlannerNameSpace
{
    public enum RemoveListCriterion
    {
        IfOnList,
        IfNotOnList,
    }

    public class Utils
    {
        public const int WorkHoursPerDay = 8;

        static Utils()
        {
            CompileHolidays();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an integer representation of the given value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetIntValue(object value)
        {
            if (value == null)
            {
                return 0;
            }

            if (value.GetType() == typeof(int))
            {
                return (int)value;
            }

            if (value.GetType() == typeof(string))
            {
                int intValue;
                if (Int32.TryParse(value as string, out intValue))
                {
                    return intValue;
                }
            }

            return 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an integer representation of the given value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static long GetLongValue(object value)
        {
            if (value == null)
            {
                return 0;
            }

            if (value.GetType() == typeof(long))
            {
                return (long)value;
            }

            if (value.GetType() == typeof(string))
            {
                long longValue;
                if (Int64.TryParse(value as string, out longValue))
                {
                    return longValue;
                }
            }

            return 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a double representation of the given value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static double GetDoubleValue(object value)
        {
            if (value == null)
            {
                return 0;
            }

            if (value.GetType() == typeof(double))
            {
                return (double)value;
            }

            if (value.GetType() == typeof(string))
            {
                double doubleValue;
                if (Double.TryParse(value as string, out doubleValue))
                {
                    return doubleValue;
                }
            }

            return 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the boolean value of the specified field for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool GetBoolValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            string strValue = GetStringValue(value);
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return false;
            }

            bool boolValue;
            if (!Boolean.TryParse(strValue, out boolValue))
            {
                return false;
            }

            return boolValue;
        }

        public static bool IsValidNumber(object value, int lowerRange, int upperRange)
        {
            if (value == null)
            {
                return false;
            }

            double number;
            if (value.GetType() == typeof(double))
            {
                number = (double)value;
            }

            else if (!Double.TryParse(value as string, out number))
            {
                return false;
            }

            return (number >= lowerRange && number <= upperRange);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a string representation of the given value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetStringValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a date representation of the given value.  If the value can't be converted
        /// to a valid DateTime object, a default DateTime object will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DateTime GetDateTimeValue(object value)
        {
            DateTime? datetime = GetNullableDateTimeValue(value);
            if (datetime == null)
            {
                return default(DateTime);
            }

            return datetime.Value;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a date representation of the given value.  If the value can't be converted
        /// to a valid DateTime object, null will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static Nullable<DateTime> GetNullableDateTimeValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            // If the given value is already a DateTime, we're good to go.
            if (value.GetType() == typeof(DateTime))
            {
                return (DateTime)value;
            }

            if (value.GetType() == typeof(string))
            {
                DateTime dt;
                if (DateTime.TryParse(value as string, out dt))
                {
                    return dt;
                }
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a trimmed string representation of the given value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetTrimmedStringValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ToString().Trim();
        }

        public static string GetSubstring2(string str, int idx, char separator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            // If there's no separator in the given string, treat the whole string as index 0
            if (str.IndexOf(separator) < 0)
            {
                if (idx == 0)
                {
                    return str;
                }
                else
                {
                    return null;
                }
            }

            if (idx == 0)
            {
                return str.Substring(0, str.IndexOf(separator));
            }
            else if (idx == 1)
            {
                return str.Substring(str.IndexOf(separator) + 1);
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a string potentially consisting of substrings separated by the given
        /// character, returns the substring at the given index.  If no string is found at
        /// that index, null will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetSubstring(string str, int idx, char separator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            string[] substrings = str.Split(separator);
            if (substrings.Length <= idx)
            {
                return null;
            }

            return substrings[idx];
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a string potentially consisting of substrings separated by the given 
        /// character, this routine replaces the current value at the given index with the
        /// given new value.  If a negative value is given for idx, the value will instead
        /// be appended appropriately to the string.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string SetSubstring(string str, string value, int idx, char separator)
        {
            if (idx < 0)
            {
                return AddSubstring(str, value, separator);
            }

            string[] substrings = string.IsNullOrWhiteSpace(str) ? null : str.Split(separator);
            int incomingCount = substrings == null ? 0 : substrings.Length;

            int totalFields = incomingCount;
            if (idx >= totalFields)
            {
                totalFields = idx + 1;
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;
            while (i < totalFields)
            {
                if (i == idx)
                {
                    sb.Append(value);
                }
                else
                {
                    if (i < incomingCount)
                    {
                        sb.Append(substrings[i]);
                    }
                }

                i++;
                if (i < totalFields)
                {
                    sb.Append(separator);
                }

            }

            return sb.ToString();
        }

        public static string ClearSubstring(string str, int idx, char separator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            if (idx < 0)
            {
                return str;
            }

            string[] substrings = string.IsNullOrWhiteSpace(str) ? null : str.Split(separator);
            int totalFields = substrings.Length;
            if (idx >= totalFields)
            {
                return str;
            }

            if (totalFields == 1)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string substring in substrings)
            {
                if (idx != i)
                {
                    sb.Append(substring);
                    sb.Append(separator);
                }

                i++;
            }

            string fullstring = sb.ToString();
            fullstring = fullstring.TrimEnd('^');
            return fullstring;
        }

        public static string AddSubstring(string str, string value, char separator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return value;
            }

            StringBuilder sb = new StringBuilder(str);
            sb.Append(separator);
            sb.Append(value);
            return sb.ToString();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a string potentially consisting of substrings separated by the given 
        /// character, this routine returns the index of the substring that matches the
        /// given value.  If no match is found, a negative value is returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int FindSubstring(string str, string valueToFind, char separator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return -1;
            }

            int idx = 0;
            string[] substrings = str.Split(separator);
            foreach (string substring in substrings)
            {
                if (StringsMatch(substring, valueToFind))
                {
                    return idx;
                }

                idx++;
            }

            return -1;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a string containing a bug ID in parentheses, returns the ID.  If no ID is 
        /// found, zero will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int ExtractParensBugID(object bugReference)
        {
            string strBugReference = bugReference as string;
            if (string.IsNullOrEmpty(strBugReference))
            {
                return 0;
            }

            int idxOpenParen = strBugReference.LastIndexOf('(');
            if (idxOpenParen >= 0)
            {
                int idxCloseParen = strBugReference.LastIndexOf(')');
                if (idxCloseParen > idxOpenParen)
                {
                    int idxBugID = idxOpenParen + 1;
                    string strBugID = strBugReference.Substring(idxBugID, idxCloseParen - idxBugID);
                    try
                    {
                        return Convert.ToInt32(strBugID);
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a full path to a temp file suitable for serializing and unserializing
        /// schedule definition data.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetFullPathToTempFile(string fileName)
        {
            string fullpath = Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            DirectoryInfo di = Directory.CreateDirectory(fullpath);
            return Path.Combine(fullpath, fileName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true the given string contains any chars that are invalid for filenames.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool ContainsInvalidFileNameChars(string str)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (str.IndexOfAny(invalidChars) >= 0)
            {
                return true;
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        // Returns false if the keypress represented in KeyEventArgs isn't acceptable for
        // a control that wants to accept only numeric input.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool IsKeypressNonNumeric(KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Escape || e.Key == Key.Enter)
                return false;

            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                return false;

            if (e.Key >= Key.D0 && e.Key <= Key.D9)
                return false;

            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Serializes the given object to the local storage file specified by fullPath.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void SerializeObject(string fullPath, object objToSerialize)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, objToSerialize);
            stream.Close();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Unserializes an object of the given type from the file specified in fullPath.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static ObjectType UnserializeObject<ObjectType>(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                ObjectType obj = (ObjectType)formatter.Deserialize(stream);
                stream.Close();
                stream.Dispose();
                return obj;
            }

            return default(ObjectType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Serializes the given object to the local storage file specified by fullPath.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void SerializeObjectToXml<ObjectType>(string fullPath, object objToSerialize)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObjectType));
            TextWriter writer = new StreamWriter(fullPath);

            serializer.Serialize(writer, objToSerialize);
            writer.Close();
            writer.Dispose();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Unserializes an object of the given type from the file specified in fullPath.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static ObjectType UnserializeObjectFromXml<ObjectType>(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObjectType));
                TextReader reader = new StreamReader(fullPath);
                ObjectType obj = (ObjectType)serializer.Deserialize(reader);
                reader.Close();

                return obj;
            }

            return default(ObjectType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given strings match, ignoring case and leading and trailing 
        /// whitespace, and treating a null string as equivalent to an empty string.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool StringsMatch(string a, string b)
        {
            if (String.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
                return true;

            if (a == null || b == null)
                return false;

            return String.Compare(a, b, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the given value in universal time format.  If the value is already 
        /// universal, the format will be unchanged.
        /// </summary>
        /// 
        /// <remarks>
        /// Assumes that if the DateTime kind is unspecified, the current format is
        /// universal.
        /// </remarks>
        //------------------------------------------------------------------------------------
        public static DateTime GetValueAsUniversalTime(object value)
        {
            DateTime date = (DateTime)value;
            if (date.Kind == DateTimeKind.Local)
            {
                return DateTime.SpecifyKind(date.ToUniversalTime(), DateTimeKind.Utc);
            }

            if (date.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }

            return date;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the given value in local time format.  If the value is already 
        /// local, the format will be unchanged.
        /// </summary>
        /// 
        /// <remarks>
        /// Assumes that if the DateTime kind is unspecified, the current format is
        /// universal.
        /// </remarks>
        //------------------------------------------------------------------------------------
        public static DateTime GetValueAsLocalTime(object value)
        {
            DateTime date = (DateTime)value;
            if (date.Kind == DateTimeKind.Utc || date.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(date.ToLocalTime(), DateTimeKind.Local);
            }

            return date;
        }

        public static DateTime GetValueAsLocalDate(object value)
        {
            DateTime dateTime = GetValueAsLocalTime(value);
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the value object associated with the given field is null.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool IsFieldValueNull(ProductStudio.Field field)
        {
            return field.Value is System.DBNull;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given value is non-empty.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool IsValueSet(object value)
        {
            if (value == null)
                return false;

            return !String.IsNullOrWhiteSpace(value.ToString());
        }

        public static void ShowWaitCursor()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        public static void ShowDefaultCursor()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        public static Dictionary<string, DateTime> Holidays;

        public static void ShowExceptionMessage(Exception e, string message, string caption)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine();
            sb.AppendLine(e.Message);
            UserMessage.Show(sb.ToString(), caption);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingDays
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetWorkingDays(DateTime startDate, DateTime endDate)
        {
            return GetNetWorkingDays(startDate, endDate, new AsyncObservableCollection<OffTimeItem>());
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingDays
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetWorkingDays(DateTime startDate, DateTime endDate, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            int netWorkingDays = 0;
            DateTime date = startDate;
            while (date <= endDate)
            {
                if (IsWorkDay(date))
                {
                    bool isOffTimeDate = false;
                    if (offTimeItems.Count > 0)
                    {
                        foreach (OffTimeItem offTimeItem in offTimeItems)
                        {
                            if (date >= offTimeItem.StartDate && date <= offTimeItem.EndDate)
                            {
                                isOffTimeDate = true;
                                break;
                            }
                        }
                    }

                    if (!isOffTimeDate)
                    {
                        netWorkingDays++;
                    }
                }

                date = date.AddDays(1);
            }

            return netWorkingDays;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingDays
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetTotalDays(DateTime startDate, DateTime endDate)
        {
            int totalDays = 0;
            DateTime date = startDate;
            while (date <= endDate)
            {
                totalDays++;
                date = date.AddDays(1);
            }

            return totalDays;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingDays
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetOffDays(DateTime startDate, DateTime endDate, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            int netOffDays = 0;
            DateTime date = startDate;
            while (date <= endDate)
            {
                if (IsWorkDay(date))
                {
                    bool isOffTimeDate = false;

                    if (offTimeItems.Count > 0)
                    {
                        foreach (OffTimeItem offTimeItem in offTimeItems)
                        {
                            if (date >= offTimeItem.StartDate && date <= offTimeItem.EndDate)
                            {
                                isOffTimeDate = true;
                                break;
                            }
                        }
                    }

                    if (isOffTimeDate)
                    {
                        netOffDays++;
                    }
                }

                date = date.AddDays(1);
            }

            return netOffDays;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingHours - Returns the total number of working hours available between
        /// the two given days, taking into account weekends and company holidays.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetWorkingHours(DateTime startDate, DateTime endDate)
        {
            return GetNetWorkingHours(startDate, endDate, new AsyncObservableCollection<OffTimeItem>());
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingHours - Returns the total number of working hours available between
        /// the two given days, taking into account weekends and company holidays.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetWorkingHours(DateTime startDate, DateTime endDate, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            int netWorkingDays = GetNetWorkingDays(startDate, endDate, offTimeItems);
            return netWorkingDays * WorkHoursPerDay;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Starting with the given date, returns the next date going forward in time that is
        /// a working day (if the given date *is* a working day, then that same date will be
        /// returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DateTime GetNextWorkingDay(DateTime fromDate)
        {
            DateTime date = fromDate;
            for(;;)
            {
                if (IsWorkDay(date))
                {
                    return date;
                }

                date = date.AddDays(1);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Starting with the given date, returns the next date going forward in time that is
        /// a working day (if the given date *is* a working day, then that same date will be
        /// returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DateTime GetNextWorkingDay(DateTime fromDate, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            DateTime date = fromDate;
            for (; ; )
            {
                if (IsWorkDay(date, offTimeItems))
                {
                    return date;
                }

                date = date.AddDays(1);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Worker function that sets the landing date for the given backlog item, based on
        /// the given starting date, and the given estimate of the number of working days to
        /// complete the job.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DateTime AddWorkingDays(DateTime startingDate, int daysToAdd, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            DateTime landingDate = startingDate;
            while (daysToAdd > 0)
            {
                landingDate = landingDate.AddDays(1);
                landingDate = Utils.GetNextWorkingDay(landingDate, offTimeItems);
                daysToAdd--;
            }

            return landingDate;
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given date lands on a company work day.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool IsWorkDay(DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday && !IsHoliday(date))
            {
                return true;
            }

            return false;
        }

        public static bool IsWorkDay(DateTime date, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            if (IsWorkDay(date))
            {
                if (offTimeItems.Count > 0)
                {
                    foreach (OffTimeItem offTimeItem in offTimeItems)
                    {
                        if (date >= offTimeItem.StartDate && date <= offTimeItem.EndDate)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsHoliday(DateTime date)
        {
            return Holidays.ContainsKey(date.ToShortDateString());
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a long string of arbitrary length, this method returns a string capped at
        /// a length specified by the MaxShortStringLength global (adding an ellipse to the
        /// end as appropriate).
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetStandardShortString(string fullstring)
        {
            if (string.IsNullOrWhiteSpace(fullstring))
            {
                return "";
            }

            if (fullstring.Length > Globals.MaxShortStringLength)
            {
                fullstring = fullstring.Substring(0, Globals.MaxShortStringLength) + "...";
            }

            return fullstring;
        }

        public static Color GetStatusColor(int itemsOnTrack, int itemsNotOnTrack)
        {
            int total = itemsOnTrack + itemsNotOnTrack;
            if (total <=0)
            {
                return Colors.Yellow;
            }

            double pctOnTrack = ((double) itemsOnTrack / (double) total) * 100;
            if (pctOnTrack < 10)
            {
                return Colors.Red;
            }
            else if (pctOnTrack < 20)
            {
                return Colors.Salmon;
            }
            else if (pctOnTrack < 30)
            {
                return Colors.LightSalmon;
            }

            else if (pctOnTrack < 40)
            {
                return Colors.Pink;
            }

            else if (pctOnTrack < 50)
            {
                return Colors.LightPink;
            }

            else if (pctOnTrack < 60)
            {
                return Colors.Yellow;
            }

            else if (pctOnTrack < 70)
            {
                return Colors.LightYellow;
            }

            else if (pctOnTrack < 80)
            {
                return Colors.YellowGreen;
            }

            else if (pctOnTrack < 90)
            {
                return Colors.LightGreen;
            }

            else
            {
                return Colors.Green;
            }
        }

        public static SolidColorBrush GetWorkCapacityFillColor(int workHoursRemaining, int workHoursAvailable)
        {
            const byte MaxRed = 235;
            const byte MaxGreen = 235;

            const double GreenThreshold = 0.70;
            const double YellowThreshold = 1.00;
            const double RedThreshold = 1.10;

            SolidColorBrush fillBrush = new SolidColorBrush();

            double remaining = workHoursRemaining;
            double available = workHoursAvailable;

            double fillPct = available <= 0 ? 2 : remaining / available;

            if (remaining == 0)
            {
                fillBrush.Color = Colors.Green;
            }
            else if (fillPct <= GreenThreshold)
            {
                fillBrush.Color = Color.FromRgb(0, MaxGreen, 0);
            }
            else if (fillPct < YellowThreshold)
            {
                int steps = (int) ((YellowThreshold - GreenThreshold) * 100);
                int perStep = MaxRed / steps;
                byte redComponent = (byte) ((fillPct - GreenThreshold) * 100 * perStep);
                fillBrush.Color = Color.FromRgb(redComponent, MaxGreen, 0);
            }
            else if (fillPct == YellowThreshold)
            {
                fillBrush.Color = Color.FromRgb(MaxRed, MaxGreen, 0);
            }
            else if (fillPct <= RedThreshold)
            {
                int steps = (int) ((RedThreshold - YellowThreshold) * 100);
                int perStep = MaxGreen / steps;
                byte greenComponent = (byte) ((RedThreshold - fillPct) * 100 * perStep);
                fillBrush.Color = Color.FromRgb(MaxRed, greenComponent, 0);
            }
            else
            {
                fillBrush.Color = Color.FromRgb(MaxRed, 0, 0);
            }

            return fillBrush;
        }

        public static BitmapSource GetBitmapSourceFromStream(Stream stream)
        {
            JpegBitmapDecoder decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            BitmapSource source = decoder.Frames[0];
            source.Freeze();
            return source;

        }

        public static void FitWindowToScreen(Window window)
        {
            if (window.Height > SystemParameters.FullPrimaryScreenHeight || window.Width > SystemParameters.FullPrimaryScreenWidth)
            {
                window.Height = SystemParameters.FullPrimaryScreenHeight - 10;
                window.Width = SystemParameters.FullPrimaryScreenWidth - 10;
            }

        }

        public static void SetCustomSorting(ItemsControl itemsControl, BaseSort comparer, ListSortDirection direction)
        {
            ListCollectionView sortView = (ListCollectionView)(CollectionViewSource.GetDefaultView(itemsControl.ItemsSource));
            comparer.SetSortDirection(direction);
            sortView.CustomSort = comparer;
        }

        public static void SetCustomSortingForColumn(ItemsControl itemsControl, BaseSort comparer, DataGridSortingEventArgs e)
        {
            ListCollectionView sortView = (ListCollectionView)(CollectionViewSource.GetDefaultView(itemsControl.ItemsSource));
            if (e.Column.SortDirection == ListSortDirection.Ascending)
            {
                e.Column.SortDirection = ListSortDirection.Descending;
                comparer.SetSortDirection(ListSortDirection.Descending);
            }
            else
            {
                e.Column.SortDirection = ListSortDirection.Ascending;
                comparer.SetSortDirection(ListSortDirection.Ascending);
            }

            sortView.CustomSort = comparer;
            e.Handled = true;
        }


        public static void ClearSortCriteria(ItemsControl itemsControl)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(itemsControl.ItemsSource);
            view.SortDescriptions.Clear();
        }

        public static void AddSortCriteria(ItemsControl itemsControl, string sortColumn)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(itemsControl.ItemsSource);
            view.SortDescriptions.Add(new SortDescription(sortColumn, ListSortDirection.Ascending));
        }

        public static void SetSortCriteria(ItemsControl itemsControl, string sortColumn)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(itemsControl.ItemsSource);
            SetSortCriteria(view, sortColumn);
        }

        public static void SetSortCriteria(CollectionView view, string sortColumn)
        {
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(sortColumn, ListSortDirection.Ascending));
        }

        public static AsyncObservableCollection<T> GetItems<T>(AsyncObservableCollection<T> collection, DummyItemType collectionType) where T : StoreItem, new()
        {
            AsyncObservableCollection<T> items = collection.ToCollection();
            items.Sort((x, y) => x.Title.CompareTo(y.Title));

            if (collectionType == DummyItemType.NoneType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.NoneType);
                items.Insert(0, item);
            }

            if (collectionType == DummyItemType.AllType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.AllType);
                items.Insert(0, item);
            }

            return items;
        }

        public static AsyncObservableCollection<T> GetItems<T>(AsyncObservableCollection<T> collection, DummyItemType collectionType, string sortPropName) where T : StoreItem, new()
        {
            AsyncObservableCollection<T> items = collection.ToCollection();

            if (collectionType == DummyItemType.NoneType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.NoneType);
                items.Add(item);
            }

            if (collectionType == DummyItemType.AllType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.AllType);
                items.Add(item);
            }

            ItemPropertySort<T> itemComparer = new ItemPropertySort<T>(sortPropName, System.ComponentModel.ListSortDirection.Ascending);
            items.Sort((x, y) => itemComparer.Compare(x, y));

            return items;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reads a string that is assumed to be serialized text from the specified property 
        /// field of the given StoreItem, and attempts to unserialized that text to an object
        /// of the specified type.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static T UnserializeFromItemProperty<T>(StoreItem item, string propName, [CallerMemberName] string publicPropName = "") where T : new()
        {
            T unserializedObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string objectText = item.GetStringValue(propName, publicPropName);
            if (!string.IsNullOrWhiteSpace(objectText))
            {
                StringReader stringReader = new StringReader(objectText);

                try
                {
                    unserializedObject = (T)serializer.Deserialize(stringReader);
                }

                catch
                {
                    unserializedObject = new T();
                }
            }
            else
            {
                unserializedObject = new T();
            }

            return unserializedObject;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Serializes the object of the given type, stores the serialized text in the
        /// specified property field of the given StoreItem, and commits the change to the
        /// store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void SerializeToItemProperty<T>(StoreItem item, string propName, T objectToSerialize, [CallerMemberName] string publicPropName = "")
        {
            item.BeginSaveImmediate();

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, objectToSerialize);
            string serializedText = stringWriter.ToString();
            item.SetStringValue(propName, serializedText, publicPropName);

            item.SaveImmediate();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the string representation of the given enumeration value - if the enum
        /// value has underscore chars in it, they will be replaced by spaces.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string EnumToString<T>(T enumVal)
        {
            string enumText = enumVal.ToString();
            enumText = enumText.Replace("aa", "<");
            enumText = enumText.Replace("zz", ">");
            enumText = enumText.Replace('_', ' ');
            return enumText;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the enum value represented by the given string, for the specified enum
        /// type, with any spaces in the string replaced by underscores.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static T StringToEnum<T>(string enumText)
        {
            if (enumText == null)
            {
                return default(T);
            }

            try
            {
                enumText = enumText.Replace("<", "aa");
                enumText = enumText.Replace(">", "zz");
                enumText = enumText.Replace(' ', '_');
                T enumVal = (T)Enum.Parse(typeof(T), enumText, true);
                return enumVal;
            }

            catch
            {
                return default(T);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an array of strings representing all the values in the specified enum
        /// type.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static AsyncObservableCollection<string> GetEnumValues<T>(bool excludeWildcardValues = false)
        {
            Array enumValues = typeof(T).GetEnumValues();
            AsyncObservableCollection<string> values = new AsyncObservableCollection<string>();
            foreach (T enumVal in enumValues)
            {
                string enumText = EnumToString<T>(enumVal);
                if (!excludeWildcardValues || (!enumText.StartsWith("<") && !enumText.StartsWith("aa")))
                {
                    values.Add(enumText);
                }
            }

            return values;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a MenuItem to the given menu, using the specified parameters.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void AddContextMenuItem(ContextMenu menu, string title, string imageName, RoutedEventHandler handler)
        {
            MenuItem item = new MenuItem();
            item.Header = title;
            item.Click += handler;
            item.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri(@"/images/" + imageName, UriKind.RelativeOrAbsolute))
            };

            menu.Items.Add(item);
        }

        public static void OpenContextMenu(ContextMenu menu)
        {
            if (menu != null)
            {
                FrameworkElement frameworkElement = null;
                IInputElement element = Mouse.DirectlyOver;
                if (element != null && element is FrameworkElement)
                {
                    frameworkElement = (FrameworkElement)element;
                }

                menu.PlacementTarget = frameworkElement;
                menu.IsOpen = true;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Puts up a standard FilePicker dialog, allowing the user to select a group of files
        /// from the file system.  If the user elects to cancel the operation, null will be
        /// returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static IList SelectFiles()
        {
            Microsoft.Win32.OpenFileDialog addFilesDialog = new Microsoft.Win32.OpenFileDialog();
            addFilesDialog.CheckFileExists = true;
            addFilesDialog.CheckPathExists = true;
            addFilesDialog.Multiselect = true;
            addFilesDialog.Filter = "All Files|*.*";
            Nullable<bool> result = addFilesDialog.ShowDialog();
            if (result == true)
            {
                return addFilesDialog.FileNames;
            }

            return null;
        }

        public static Brush GetBrush(byte red, byte green, byte blue)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromRgb(red, green, blue);
            return brush;
        }

        public static string GetExpressionName<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("The body must be a member expression");
            }

            return body.Member.Name;
        }

        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Compiles the list of MS holidays.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        private static void CompileHolidays()
        {
            if (Holidays == null)
            {
                Holidays = new Dictionary<string, DateTime>();

                // All company holidays for 2013
                AddHoliday(new DateTime(2013, 1, 1));
                AddHoliday(new DateTime(2013, 5, 27));
                AddHoliday(new DateTime(2013, 7, 4));
                AddHoliday(new DateTime(2013, 9, 2));
                AddHoliday(new DateTime(2013, 11, 28));
                AddHoliday(new DateTime(2013, 11, 29));
                AddHoliday(new DateTime(2013, 12, 24));
                AddHoliday(new DateTime(2013, 12, 25));

                // 2014
                AddHoliday(new DateTime(2014, 1, 1));
                AddHoliday(new DateTime(2014, 5, 26));
                AddHoliday(new DateTime(2014, 7, 4));
                AddHoliday(new DateTime(2014, 9, 1));
                AddHoliday(new DateTime(2014, 11, 27));
                AddHoliday(new DateTime(2014, 11, 28));
                AddHoliday(new DateTime(2014, 12, 24));
                AddHoliday(new DateTime(2014, 12, 25));
            }
        }

        private static void AddHoliday(DateTime holiday)
        {
            Holidays.Add(holiday.ToShortDateString(), holiday);
        }

    }
}
