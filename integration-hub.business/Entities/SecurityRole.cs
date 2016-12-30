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
    public class SecurityRole : IDatabaseEntity
    {
        public Guid SecurityRoleId { get; private set; }
        public Guid ModifiedBy { get; private set; }
        public Guid CreatedBy { get; private set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime ModifiedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        public int StateCode { get; private set; }

        private bool ExistingRecord;

        public SecurityRole()
        {
            ExistingRecord = false;

            this.SecurityRoleId = Guid.NewGuid();
            this.StateCode = 0;
        }

        public SecurityRole(Guid _securityRoleId)
        {
            this.SecurityRoleId = _securityRoleId;

            DataTable _dataTable = GetDetailsFromId();
            DataRow _dataRow = _dataTable.Rows[0];

            ExistingRecord = true;

            Guid _modifiedBy;
            Guid _createdBy;
            DateTime _modifiedOn;
            DateTime _createdOn;

            int _stateCode;

            this.Name = _dataRow["Name"].ToString();
            this.Description = _dataRow["Description"].ToString();

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

        private DataTable GetDetailsFromId()
        {
            // Prepare parameter list for stored procedure
            Hashtable parameterList = new Hashtable();
            parameterList.Add("@SecurityRoleId", this.SecurityRoleId);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_SecurityRoleGetDetailsFromId]", parameterList);
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
            parameterList.Add("@SecurityRoleId", this.SecurityRoleId);
            parameterList.Add("@UserId", _userId);
            parameterList.Add("@Name", this.Name);
            parameterList.Add("@Description", this.Description);
            parameterList.Add("@StateCode", this.StateCode);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureNonQuery("[dbo].[usp_SecurityRoleInsert]", parameterList);
        }

        private Int32 Update(Guid _userId)
        {
            // Prepare parameter list for stored procedure
            Hashtable parameterList = new Hashtable();
            parameterList.Add("@SecurityRoleId", this.SecurityRoleId);
            parameterList.Add("@UserId", _userId);
            parameterList.Add("@Name", this.Name);
            parameterList.Add("@Description", this.Description);
            parameterList.Add("@StateCode", this.StateCode);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureNonQuery("[dbo].[usp_SecurityRoleUpdate]", parameterList);
        }

        private Int32 Delete()
        {
            // Prepare parameter list for stored procedure
            Hashtable parameterList = new Hashtable();
            parameterList.Add("@SecurityRoleId", this.SecurityRoleId);

            // Execute stored procedure and return datatable result
            return Database.DataIntegrationHub.ExecuteStoredProcedureNonQuery("[dbo].[usp_SecurityRoleDelete]", parameterList);
        }

        public static List<SecurityRole> AllSecurityRoles()
        {
            List<SecurityRole> _list = new List<SecurityRole>();

            DataTable _dataTable = Database.DataIntegrationHub.ExecuteStoredProcedureQuery("[dbo].[usp_SecurityRoleGetAll]");

            foreach (DataRow _dataRow in _dataTable.Rows)
            {
                Guid _securityRoleId;
                StringParser.Parse(_dataRow["SecurityRoleId"].ToString(), out _securityRoleId);
                SecurityRole _securityRole = new SecurityRole(_securityRoleId);
                _list.Add(_securityRole);
            }

            return _list;
        }
    }
}
