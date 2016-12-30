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
    public class UserSecurityRole : IDatabaseEntity
    {
        public Guid UserSecurityRoleId { get; private set; }
        public Guid UserId { get; set; }
        public Guid SecurityRoleId { get; set; }
        public Guid ModifiedBy { get; private set; }
        public Guid CreatedBy { get; private set; }

        public DateTime ModifiedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        public int StateCode { get; set; }

        private bool ExistingRecord;

        public UserSecurityRole()
        {
            ExistingRecord = false;

            this.UserSecurityRoleId = Guid.NewGuid();
            this.StateCode = 0;
        }

        public UserSecurityRole(Guid _userSecurityRoleId)
        {
            this.UserSecurityRoleId = _userSecurityRoleId;

            DataTable _dataTable = GetDetailsFromId();
            DataRow _dataRow = _dataTable.Rows[0];

            ExistingRecord = true;

            Guid _userId;
            Guid _securityRoleId;
            Guid _modifiedBy;
            Guid _createdBy;
            DateTime _modifiedOn;
            DateTime _createdOn;

            int _stateCode;

            StringParser.Parse(_dataRow["UserId"].ToString(), out _userId);
            StringParser.Parse(_dataRow["SecurityRoleId"].ToString(), out _securityRoleId);
            StringParser.Parse(_dataRow["ModifiedBy"].ToString(), out _modifiedBy);
            StringParser.Parse(_dataRow["CreatedBy"].ToString(), out _createdBy);
            StringParser.Parse(_dataRow["ModifiedOn"].ToString(), out _modifiedOn);
            StringParser.Parse(_dataRow["CreatedOn"].ToString(), out _createdOn);
            StringParser.Parse(_dataRow["StateCode"].ToString(), out _stateCode);

            this.UserId = _userId;
            this.SecurityRoleId = _securityRoleId;
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
            parameterList.Add("@UserSecurityRoleId", this.UserSecurityRoleId);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_UserSecurityRoleGetDetailsFromId]", parameterList);
        }

        /// <summary>
        /// Update or insert this instance into the SecurityRole table.
        /// </summary>
        /// <param name="_userId">The UserId that modified or created the record.</param>
        public void SaveToDatabase(Guid _userId)
        {
            if (ExistingRecord)
            {
                this.Update(_userId);
            }
            else
            {
                this.Insert(_userId);
                this.ExistingRecord = true;
            }
        }

        public void DeleteFromDatabase()
        {
            this.Delete();
            this.ExistingRecord = false;
        }

        private Int32 Insert(Guid _userId)
        {
            // Prepare parameter list for stored procedure
            Hashtable parameterList = new Hashtable();
            parameterList.Add("@UserSecurityRoleId", this.UserSecurityRoleId);
            parameterList.Add("@CurrentUserId", _userId);
            parameterList.Add("@UserId", this.UserId);
            parameterList.Add("@SecurityRoleId", this.SecurityRoleId);
            parameterList.Add("@StateCode", this.StateCode);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureNonQuery("[dbo].[usp_UserSecurityRoleInsert]", parameterList);
        }

        private Int32 Update(Guid _userId)
        {
            // Prepare parameter list for stored procedure
            Hashtable parameterList = new Hashtable();
            parameterList.Add("@UserSecurityRoleId", this.UserSecurityRoleId);
            parameterList.Add("@CurrentUserId", _userId);
            parameterList.Add("@UserId", this.UserId);
            parameterList.Add("@SecurityRoleId", this.SecurityRoleId);
            parameterList.Add("@StateCode", this.StateCode);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureNonQuery("[dbo].[usp_UserSecurityRoleUpdate]", parameterList);
        }

        private Int32 Delete()
        {
            // Prepare parameter list for stored procedure
            Hashtable parameterList = new Hashtable();
            parameterList.Add("@UserSecurityRoleId", this.UserSecurityRoleId);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureNonQuery("[dbo].[usp_UserSecurityRoleDelete]", parameterList);
        }

        public static List<UserSecurityRole> AllSecurityRoles()
        {
            List<UserSecurityRole> _list = new List<UserSecurityRole>();

            DataTable _dataTable = Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_UserSecurityRoleGetAll]");

            foreach (DataRow _dataRow in _dataTable.Rows)
            {
                Guid _userSecurityRoleId;
                StringParser.Parse(_dataRow["UserSecurityRoleId"].ToString(), out _userSecurityRoleId);
                UserSecurityRole _userSecurityRole = new UserSecurityRole(_userSecurityRoleId);
                _list.Add(_userSecurityRole);
            }

            return _list;
        }

        public static DataTable GetAllSecurityRoles()
        {
            return Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_UserSecurityRoleGetAll]");
        }

        public static List<SecurityRole> GetAssociatedFromUser(User _user)
        {
            List<SecurityRole> _list = new List<SecurityRole>();

            DataTable _dataTable = GetAssociatedFromUserId(_user.UserId);

            foreach (DataRow _dataRow in _dataTable.Rows)
            {
                Guid _securityRoleId;
                StringParser.Parse(_dataRow["SecurityRoleId"].ToString(), out _securityRoleId);
                SecurityRole _securityRole = new SecurityRole(_securityRoleId);
                _list.Add(_securityRole);
            }

            return _list;
        }

        private static DataTable GetAssociatedFromUserId(Guid _userId)
        {
            // Prepare parameter list for stored procedure
            Hashtable parameterList = new Hashtable();
            parameterList.Add("@UserId", _userId);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_UserSecurityRoleGetAssociatedFromUser]", parameterList);
        }
    }
}
