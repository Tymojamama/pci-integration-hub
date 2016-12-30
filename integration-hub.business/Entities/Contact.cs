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
    public class Contact : IDatabaseEntity
    {
        #region PublicMembers

        public Guid ContactId { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid ModifiedBy { get; private set; }
        public Guid CreatedBy { get; private set; }

        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string EmailAddress { get; private set; }
        public string BusinessPhone { get; private set; }

        public DateTime ModifiedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        public int StateCode { get; private set; }

        #endregion

        #region PrivateMembers

        private bool ExistingRecord;

        #endregion

        public Contact(Guid _contactId)
        {
            this.ContactId = _contactId;

            ExistingRecord = true;

            DataTable _dataTable = GetDetailsFromId();
            DataRow _dataRow = _dataTable.Rows[0];

            Guid _customerId;
            Guid _modifiedBy;
            Guid _createdBy;
            DateTime _modifiedOn;
            DateTime _createdOn;

            int _stateCode;

            this.FirstName = _dataRow["FirstName"].ToString();
            this.MiddleName = _dataRow["MiddleName"].ToString();
            this.LastName = _dataRow["LastName"].ToString();
            this.EmailAddress = _dataRow["EmailAddress"].ToString();
            this.BusinessPhone = _dataRow["BusinessPhone"].ToString();

            StringParser.Parse(_dataRow["CustomerId"].ToString(), out _customerId);
            StringParser.Parse(_dataRow["ModifiedBy"].ToString(), out _modifiedBy);
            StringParser.Parse(_dataRow["CreatedBy"].ToString(), out _createdBy);
            StringParser.Parse(_dataRow["ModifiedOn"].ToString(), out _modifiedOn);
            StringParser.Parse(_dataRow["CreatedOn"].ToString(), out _createdOn);
            StringParser.Parse(_dataRow["StateCode"].ToString(), out _stateCode);

            this.CustomerId = _customerId;
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
            parameterList.Add("@ContactId", this.ContactId);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_ContactGetDetailsFromId]", parameterList);
        }
    }
}
