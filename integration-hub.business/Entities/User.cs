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
    public class User : IDatabaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid ModifiedBy { get; private set; }
        public Guid CreatedBy { get; private set; }

        public string DomainName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName { get; private set; }
        public string Title { get; private set; }
        public string MainPhone { get; private set; }
        public string EmailAddress { get; private set; }

        public DateTime ModifiedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        public int StateCode { get; private set; }

        private bool ExistingRecord;
        
        public User(Guid _userId)
        {
            this.UserId = _userId;

            ExistingRecord = true;

            DataTable _dataTable = GetDetailsFromId();
            DataRow _dataRow = _dataTable.Rows[0];

            Guid _modifiedBy;
            Guid _createdBy;
            DateTime _modifiedOn;
            DateTime _createdOn;

            int _stateCode;

            this.DomainName = _dataRow["DomainName"].ToString();
            this.FirstName = _dataRow["FirstName"].ToString();
            this.LastName = _dataRow["LastName"].ToString();
            this.FullName = FirstName + " " + LastName;
            this.Title = _dataRow["Title"].ToString();
            this.MainPhone = _dataRow["MainPhone"].ToString();
            this.EmailAddress = _dataRow["EmailAddress"].ToString();

            StringParser.Parse(_dataRow["ModifiedBy"].ToString(), out _modifiedBy);
            StringParser.Parse(_dataRow["CreatedBy"].ToString(), out _createdBy);
            StringParser.Parse(_dataRow["ModifiedOn"].ToString(), out _modifiedOn);
            StringParser.Parse(_dataRow["CreatedOn"].ToString(), out _createdOn);
            StringParser.Parse(_dataRow["StateCode"].ToString(), out _stateCode);

            this.ModifiedBy = _modifiedBy;
            this.ModifiedOn = _modifiedOn;
            this.CreatedBy = _createdBy;
            this.CreatedOn = _createdOn;
            this.StateCode = _stateCode;
        }

        public User(string _domainName)
        {
            this.DomainName = _domainName;

            ExistingRecord = true;

            DataTable _dataTable = GetDetailsFromDomainName();

            if (_dataTable.Rows.Count != 1)
                throw new Exception("The domain name did not return exactly one record.");

            DataRow _dataRow = _dataTable.Rows[0];

            Guid _userId;
            Guid _modifiedBy;
            Guid _createdBy;
            DateTime _modifiedOn;
            DateTime _createdOn;

            int _stateCode;

            this.DomainName = _dataRow["DomainName"].ToString();
            this.FirstName = _dataRow["FirstName"].ToString();
            this.LastName = _dataRow["LastName"].ToString();
            this.Title = _dataRow["Title"].ToString();
            this.MainPhone = _dataRow["MainPhone"].ToString();
            this.EmailAddress = _dataRow["EmailAddress"].ToString();

            StringParser.Parse(_dataRow["UserId"].ToString(), out _userId);
            StringParser.Parse(_dataRow["ModifiedBy"].ToString(), out _modifiedBy);
            StringParser.Parse(_dataRow["CreatedBy"].ToString(), out _createdBy);
            StringParser.Parse(_dataRow["ModifiedOn"].ToString(), out _modifiedOn);
            StringParser.Parse(_dataRow["CreatedOn"].ToString(), out _createdOn);
            StringParser.Parse(_dataRow["StateCode"].ToString(), out _stateCode);

            this.UserId = _userId;
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
            parameterList.Add("@UserId", this.UserId);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_UserGetDetailsFromId]", parameterList);
        }

        private DataTable GetDetailsFromDomainName()
        {
            // Prepare parameter list for stored procedure
            Hashtable parameterList = new Hashtable();
            parameterList.Add("@DomainName", this.DomainName);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_UserGetDetailsFromDomainName]", parameterList);
        }

        public static List<User> AllUsers()
        {
            List<User> _list = new List<User>();

            DataTable _dataTable = Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_UserGetAll]");

            foreach (DataRow _dataRow in _dataTable.Rows)
            {
                Guid _userId;
                StringParser.Parse(_dataRow["UserId"].ToString(), out _userId);
                User _user = new User(_userId);
                _list.Add(_user);
            }

            return _list;
        }

        public static List<User> ActiveUsers()
        {
            List<User> _list = new List<User>();

            DataTable _dataTable = Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_UserGetActive]");

            foreach (DataRow _dataRow in _dataTable.Rows)
            {
                Guid _userId;
                StringParser.Parse(_dataRow["UserId"].ToString(), out _userId);
                User _user = new User(_userId);
                _list.Add(_user);
            }

            return _list;
        }
    }
}
