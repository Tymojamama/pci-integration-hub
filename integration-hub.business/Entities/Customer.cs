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
    public class Customer : IDatabaseEntity
    {
        #region PublicMembers

        public Guid CustomerId { get; private set; }
        public Guid PrimaryContactId { get; private set; }
        public Guid ModifiedBy { get; private set; }
        public Guid CreatedBy { get; private set; }

        public string Name { get; private set; }
        public string MainPhone { get; private set; }
        public string Fax { get; private set; }

        public Address Address { get; private set; }

        public decimal? AssetValue { get; private set; }

        public DateTime ModifiedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        public int StateCode { get; private set; }

        #endregion

        #region PrivateMembers

        private bool ExistingRecord;

        #endregion
        
        public Customer(Guid _customerId)
        {
            this.CustomerId = _customerId;

            ExistingRecord = true;

            DataTable _dataTable = GetDetailsFromId();
            DataRow _dataRow = _dataTable.Rows[0];

            Guid _primaryContact;
            Guid _modifiedBy;
            Guid _createdBy;
            DateTime _modifiedOn;
            DateTime _createdOn;
            decimal _assetValue;

            int _stateCode;

            this.Name = _dataRow["Name"].ToString();
            this.MainPhone = _dataRow["MainPhone"].ToString();
            this.Fax = _dataRow["Fax"].ToString();

            Address = new Address()
            {
                Line1 = _dataRow["AddressLine1"].ToString(),
                Line2 = _dataRow["AddressLine2"].ToString(),
                City = _dataRow["AddressCity"].ToString(),
                State = _dataRow["AddressState"].ToString(),
                ZipCode = _dataRow["AddressZip"].ToString()
            };

            StringParser.Parse(_dataRow["PrimaryContactId"].ToString(), out _primaryContact);
            StringParser.Parse(_dataRow["AssetValue"].ToString(), out _assetValue);
            StringParser.Parse(_dataRow["ModifiedBy"].ToString(), out _modifiedBy);
            StringParser.Parse(_dataRow["CreatedBy"].ToString(), out _createdBy);
            StringParser.Parse(_dataRow["ModifiedOn"].ToString(), out _modifiedOn);
            StringParser.Parse(_dataRow["CreatedOn"].ToString(), out _createdOn);
            StringParser.Parse(_dataRow["StateCode"].ToString(), out _stateCode);

            this.PrimaryContactId = _primaryContact;
            this.AssetValue = _assetValue;
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
            parameterList.Add("@CustomerId", this.CustomerId);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_CustomerGetDetailsFromId]", parameterList);
        }
    }
}
