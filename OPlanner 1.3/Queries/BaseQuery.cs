using System;
using System.Collections.Generic;
using System.Text;

namespace PlannerNameSpace
{
    internal class QueryDefinitionException : Exception { }

    public abstract class BaseQuery
    {
        public Datastore Store { get; set; }
        public BaseQuery(Datastore store)
        {
            Store = store;
        }

        public string QueryXml
        {
            get
            {
                Initialize();
                return m_psQueryXml.ToString();
            }
        }

        private void Initialize()
        {
            if (m_psQueryXml == null)
            {
                m_psQueryXml = new StringBuilder();

                BuildQueryXML();
            }
        }

        protected abstract void BuildQueryXML();

        protected void BeginQuery()
        {
            m_psQueryXml.Append("<Query>");
        }

        protected void EndQuery()
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("</Query>");
        }

        protected void BeginAndGroup()
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("<Group GroupOperator='And'>");
        }

        protected void BeginOrGroup()
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("<Group GroupOperator='Or'>");
        }

        protected void EndGroup()
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("</Group>");
        }

        protected void AddAssignedToClause(string strGroupName)
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("<Expression Column='Assigned To' Operator='Equals' ExpandConstant='1'><String>");
            m_psQueryXml.Append(strGroupName);
            m_psQueryXml.Append("</String></Expression>");
        }

        protected void AddAssignedToGroupClause(string strGroupName)
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("<Expression Column='Assigned To' Operator='Equals' ExpandConstant='1'><String>");
            m_psQueryXml.Append(strGroupName);
            m_psQueryXml.Append("</String></Expression>");
        }

        protected void AddResolvedByClause(string strGroupName)
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("<Expression Column='Resolved By' Operator='Equals' ExpandConstant='1'><String>");
            m_psQueryXml.Append(strGroupName);
            m_psQueryXml.Append("</String></Expression>");
        }

        protected void AddClause(string strFieldName, string strOperator, string strFieldValue)
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("<Expression ");
            m_psQueryXml.Append("Column='" + strFieldName + "' ");
            m_psQueryXml.Append("Operator='" + strOperator + "'>");
            m_psQueryXml.Append("<String>" + strFieldValue + "</String>");
            m_psQueryXml.Append("</Expression>");
        }

        protected void AddIDEqualsClause(int itemID)
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("<Expression Column='ID' Operator='Equals'><Number>");
            m_psQueryXml.Append(itemID.ToString());
            m_psQueryXml.Append("</Number></Expression>");
        }

        protected void AddTreeIDUnderClause(int treeID)
        {
            if (m_psQueryXml == null) throw new QueryDefinitionException();

            m_psQueryXml.Append("<Expression Column='TreeID' FieldType='32' Operator='under'><Number>");
            m_psQueryXml.Append(treeID.ToString());
            m_psQueryXml.Append("</Number></Expression>");
        }

        protected bool AddRefreshDateFilterClause(ShouldRefresh shouldRefresh, RefreshType refreshType)
        {
            if (shouldRefresh == PlannerNameSpace.ShouldRefresh.Yes)
            {
                if (refreshType == PlannerNameSpace.RefreshType.QueryForChangedItems)
                {
                    DateTime lastRefreshTime = Utils.GetValueAsLocalTime(StoreQueryManager.Instance.LastRefreshTime);
                    string lastRefreshDate = lastRefreshTime.ToShortDateString();
                    AddClause(Datastore.PropNameChangedDate, "equalsGreater", lastRefreshDate);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        // Builds a query to retrieve all bugs assigned to those on the given list.
        protected void QueryForAssignedToList(List<string> assignedToList)
        {
            BeginQuery();
            BeginOrGroup();

            foreach (string assignedTo in assignedToList)
            {
                AddAssignedToClause(assignedTo);
            }

            EndGroup();
            EndQuery();
        }


        private StringBuilder m_psQueryXml = null;
    }
}
