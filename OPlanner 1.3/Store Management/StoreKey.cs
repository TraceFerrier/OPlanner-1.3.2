using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class OldStoreKey
    {
        public static string Get(StoreItem item)
        {
            return (Get(item.StoreID, item.ID));
        }

        public static OldStoreKey GetKey(StoreItem item)
        {
            return new OldStoreKey(item.StoreID, item.ID);
        }

        public static OldStoreKey GetKey(string key)
        {
            return !IsValidKey(key) ? null : new OldStoreKey(key);
        }

        public static string Get(DatastoreID storeID, int itemID)
        {
            return storeID.Name + "|" + itemID.ToString();
        }

        string m_key;
        public OldStoreKey(DatastoreID storeID, int itemID)
        {
            m_key = Get(storeID, itemID);
        }

        public OldStoreKey(string key)
        {
            m_key = key;
        }

        public string Get()
        {
            return m_key;
        }

        public string GetProduct()
        {
            return GetProduct(m_key);
        }

        public int GetID()
        {
            return GetID(m_key);
        }

        public static string GetProduct(string key)
        {
            return key.Substring(0, key.IndexOf('|'));
        }

        public static int GetID(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                int id;
                string strID = key.Substring(key.IndexOf('|') + 1);
                if (Int32.TryParse(strID, out id))
                {
                    return id;
                }
            }
            return 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given string represents a valid QueryKey.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool IsValidKey(string key)
        {
            if (key == null || key.Length == 0 || !key.Contains('|'))
            {
                return false;
            }

            string keyProduct = GetProduct(key);
            if (string.IsNullOrEmpty(keyProduct))
            {
                return false;
            }

            int keyID = GetID(key);
            if (keyID < 1)
            {
                return false;
            }

            return true;
        }
    }
}
