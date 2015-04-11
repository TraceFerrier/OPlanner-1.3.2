using ProductStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class CompositeValueRegistry
    {
        Dictionary<string, Dictionary<string, CompositeIdentifier>> m_registry;
        Datastore Store;

        public CompositeValueRegistry(Datastore store)
        {
            Store = store;
            m_registry = new Dictionary<string, Dictionary<string, CompositeIdentifier>>();
        }

        public void RegisterCompositeValue(string dsPropName, string publicPropName, int indexIntoComposite)
        {
            if (!m_registry.ContainsKey(dsPropName))
            {
                m_registry.Add(dsPropName, new Dictionary<string, CompositeIdentifier>());
            }

            if (!m_registry[dsPropName].ContainsKey(publicPropName))
            {
                m_registry[dsPropName].Add(publicPropName, new CompositeIdentifier { DSPropName = dsPropName, PublicPropName = publicPropName, Index = indexIntoComposite });
            }
            else
            {
                throw new ApplicationException("Duplicate registration!");
            }
        }

        public bool IsValueRegistered(string dsPropName, string publicPropName)
        {
            if (m_registry.ContainsKey(dsPropName))
            {
                if (m_registry[dsPropName].ContainsKey(publicPropName))
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetValueFromComposite(DatastoreItem dsItem, string dsPropName, string publicPropName, out object value)
        {
            value = null;
            if (!IsValueRegistered(dsPropName, publicPropName))
            {
                return false;
            }

            CompositeIdentifier compositeID = m_registry[dsPropName][publicPropName];
            string compositeText = Utils.GetStringValue(Store.GetBackingValue(dsItem, compositeID.DSPropName));
            value = Utils.GetSubstring(compositeText, compositeID.Index, '^');
            return true;
        }

        public bool GetCompositeFromValue(DatastoreItem dsItem, string dsPropName, string publicPropName, object value, out object compositeValue)
        {
            compositeValue = null;
            if (!IsValueRegistered(dsPropName, publicPropName))
            {
                return false;
            }

            CompositeIdentifier compositeID = m_registry[dsPropName][publicPropName];
            string valueText = Utils.GetStringValue(value);
            string compositeText = Utils.GetStringValue(Store.GetBackingValue(dsItem, compositeID.DSPropName));
            compositeValue = Utils.SetSubstring(compositeText, valueText, compositeID.Index, '^');
            return true;
        }

    }
}
