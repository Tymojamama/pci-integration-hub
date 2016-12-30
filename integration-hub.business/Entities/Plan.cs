using PensionConsultants.Data.Access;
using PensionConsultants.Data.Utilities;
using DataIntegrationHub.Business.Components;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace DataIntegrationHub.Business.Entities
{
    public class Plan : IDatabaseEntity
    {
        public Guid PlanId { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid ModifiedBy { get; private set; }
        public Guid CreatedBy { get; private set; }

        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Description { get; private set; }
        public string MorningstarName { get; private set; }

        public DateTime ModifiedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public DateTime? InceptionDate { get; private set; }

        public bool IsManagedPlan { get; private set; }

        public int StateCode { get; private set; }

        private bool ExistingRecord;
        
        public Plan(Guid _planId)
        {
            this.PlanId = _planId;

            ExistingRecord = true;

            DataTable _dataTable = GetDetailsFromId();
            DataRow _dataRow = _dataTable.Rows[0];

            Guid _customerId;
            Guid _modifiedBy;
            Guid _createdBy;
            DateTime _modifiedOn;
            DateTime _createdOn;
            DateTime? _inceptionDate;

            int _stateCode;

            this.Name = _dataRow["Name"].ToString();
            this.Type = _dataRow["Type"].ToString();
            this.Description = _dataRow["Description"].ToString();
            this.MorningstarName = _dataRow["MorningstarName"].ToString();
            this.IsManagedPlan = Boolean.Parse(_dataRow["IsManagedPlan"].ToString());

            StringParser.Parse(_dataRow["CustomerId"].ToString(), out _customerId);
            StringParser.Parse(_dataRow["InceptionDate"].ToString(), out _inceptionDate);
            StringParser.Parse(_dataRow["ModifiedBy"].ToString(), out _modifiedBy);
            StringParser.Parse(_dataRow["CreatedBy"].ToString(), out _createdBy);
            StringParser.Parse(_dataRow["ModifiedOn"].ToString(), out _modifiedOn);
            StringParser.Parse(_dataRow["CreatedOn"].ToString(), out _createdOn);
            StringParser.Parse(_dataRow["StateCode"].ToString(), out _stateCode);

            this.CustomerId = _customerId;
            this.InceptionDate = _inceptionDate;
            this.ModifiedBy = _modifiedBy;
            this.ModifiedOn = _modifiedOn;
            this.CreatedBy = _createdBy;
            this.CreatedOn = _createdOn;
            this.StateCode = _stateCode;
        }

        private DataTable GetDetailsFromId()
        {
            // Prepare parameter list for stored procedure
            Hashtable parameterList = new Hashtable();
            parameterList.Add("@PlanId", this.PlanId);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_PlanGetDetailsFromId]", parameterList);
        }

        public static List<Plan> Get()
        {
            var result = new List<Plan>();
            var dataTable = Database.DataIntegrationHub.ExecuteSqlQuery("SELECT [PlanId] FROM [Plan]");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var id = new Guid(dataRow["PlanId"].ToString());
                var plan = new Plan(id);
                result.Add(plan);
            }

            return result;
        }
    }
}
