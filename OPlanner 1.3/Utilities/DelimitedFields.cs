using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlannerNameSpace
{
    public class DelimitedFields : List<string>
    {
        public DelimitedFields()
        {
        }

        public DelimitedFields(char delimeter)
        {
            m_delimiter = delimeter;
        }

        public DelimitedFields(string fields, char delimeter)
        {
            m_delimiter = delimeter;
            ToList(fields);
        }

        private char m_delimiter = ';';

        public string GetField(int index)
        {
            if (index >= Count)
            {
                return null;
            }

            return this[index];
        }

        public void SetField(int index, string value)
        {
            if (index >= 0 && index < Count)
            {
                this[index] = value;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a string that consists of the contatenation of all the strings in the 
        /// list, delimited by the given char.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string str in this)
            {
                if (sb.Length > 0)
                {
                    sb.Append(m_delimiter, 1);
                }

                sb.Append(str);
            }

            return sb.ToString();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list that consists of all the strings concatenated together in
        /// delimitedString, separated by the given delimiter character.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ToList(string delimitedString)
        {
            this.Clear();

            if (delimitedString == null || delimitedString.Length == 0)
            {
                return;
            }

            if (delimitedString.IndexOf(m_delimiter) < 0)
            {
                this.Add(delimitedString);
                return;
            }

            int idxStart = 0;
            int idxEnd = 0;

            do
            {
                idxEnd = delimitedString.IndexOf(m_delimiter, idxStart, delimitedString.Length - idxStart);
                if (idxEnd >= 0)
                {
                    this.Add(delimitedString.Substring(idxStart, idxEnd - idxStart));
                    idxStart = idxEnd + 1;
                }
            } while (idxEnd >= 0);

            this.Add(delimitedString.Substring(idxStart));
        }
    }
}
