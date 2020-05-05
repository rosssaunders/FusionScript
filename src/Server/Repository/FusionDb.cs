using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Security;
using Sophis.DataAccess;

namespace RxdSolutions.FusionScript.Repository
{
    public class FusionDb
    {
        private int _userId;
        private bool _hasManageFirmRight;

        public FusionDb()
        {
            _userId = UserRightHelper.GetCurrentUserId();
            _hasManageFirmRight = UserRights.CanManageFirm();
        }

        public IEnumerable<ScriptModel> LoadAllScripts()
        {
            Dictionary<int, List<ScriptTriggerModel>> LoadTriggers()
            {
                var triggers = new Dictionary<int, List<ScriptTriggerModel>>();

                using var command = new OracleCommand();
                command.Connection = DBContext.Connection;
                command.CommandText = @"SELECT 
                                        t.ID, 
                                        t.TRIGGERID, 
                                        t.SCRIPTID, 
                                        t.TIME
                                    FROM RXD_SCRIPT_TRIGGER t
                                    INNER JOIN RXD_SCRIPT f ON t.SCRIPTID = f.ID
                                    WHERE 
                                        f.OWNERID = :ownerid 
                                        OR 
                                        f.SECURITYPERMISSION = :firmSecurityPermissionId
                                        OR
                                        :hasManageFirmRight = 1";

                command.CommandType = CommandType.Text;
                command.BindByName = true;
                command.Parameters.Add("ownerid", _userId);
                command.Parameters.Add("firmSecurityPermissionId", (int)SecurityPermission.Firm);
                command.Parameters.Add("hasManageFirmRight", Convert.ToInt32(_hasManageFirmRight));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var model = new ScriptTriggerModel();
                        model.ScriptId = reader.GetInt32(reader.GetOrdinal("SCRIPTID"));
                        model.Trigger = (Trigger)reader.GetInt32(reader.GetOrdinal("TRIGGERID"));
                        model.Id = reader.GetInt32(reader.GetOrdinal("ID"));

                        var timeIdx = reader.GetOrdinal("TIME");
                        if (!reader.IsDBNull(timeIdx))
                            model.Time = reader.GetDateTime(timeIdx);

                        if (!triggers.ContainsKey(model.ScriptId))
                            triggers.Add(model.ScriptId, new List<ScriptTriggerModel>());

                        triggers[model.ScriptId].Add(model);
                    }
                }
                
                return triggers;
            }

            var _triggers = LoadTriggers();

            using var command = new OracleCommand
            {
                Connection = DBContext.Connection,
                CommandText = @"SELECT 
                                        ID,
                                        NAME,
                                        DESCRIPTION,
                                        SCRIPT,
                                        OWNERID,
                                        LANGUAGE,
                                        SECURITYPERMISSION,
                                        ICON
                                    FROM RXD_SCRIPT f
                                    WHERE 
                                        f.SECURITYPERMISSION = :firmSecurityPermissionId
                                        OR
                                        f.OWNERID = :ownerid
                                        OR
                                        :hasManageFirmRight = 1",

                CommandType = CommandType.Text,
                BindByName = true
            };
            command.Parameters.Add("ownerid", _userId);
            command.Parameters.Add("firmSecurityPermissionId", (int)SecurityPermission.Firm);
            command.Parameters.Add("hasManageFirmRight", Convert.ToInt32(_hasManageFirmRight));

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var model = new ScriptModel();
                    PopulateModel(reader, model);

                    if (_triggers.ContainsKey(model.Id))
                    {
                        foreach (var trigger in _triggers[model.Id])
                        {
                            model.Triggers.Add(trigger);
                        }
                    }

                    yield return model;
                }
            }
        }

        public static void SaveScriptExecution(int scriptId, Trigger trigger, DateTime startedAt, DateTime endedAt, ExecutionStatus status, string results)
        {
            using var command = new OracleCommand
            {
                Connection = DBContext.Connection,
                CommandText = @"INSERT INTO RXD_SCRIPT_EXECUTION (id, scriptId, triggerId, startedAt, endedAt, status, results)  
                                        VALUES(RXD_SCRIPT_EXECUTION_ID.nextval, :scriptId, :triggerId, :startAt, :endAt, :status, :results)
                                        RETURNING id into :id",

                BindByName = true
            };
            var parameter = command.Parameters.Add("id", scriptId);
            parameter.Direction = ParameterDirection.Output;

            command.Parameters.Add("scriptId", scriptId);
            command.Parameters.Add("triggerId", (int)trigger);
            command.Parameters.Add("startAt", startedAt);
            command.Parameters.Add("endAt", endedAt);
            command.Parameters.Add("status", (int)status);
            command.Parameters.Add("results", results);

            command.CommandType = CommandType.Text;

            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected != 1)
            {
                throw new ApplicationException($"Expecting one row inserted. Got {rowsAffected}");
            }
        }

        public IEnumerable<ScriptExecutionModel> LoadScriptExecutions(int id)
        {
            using var command = new OracleCommand
            {
                Connection = DBContext.Connection,
                CommandText = @"SELECT 
                                    ID,
                                    STARTEDAT,
                                    ENDEDAT,
                                    TRIGGERID,
                                    RESULTS,
                                    STATUS,
                                    SCRIPTID
                                FROM RXD_SCRIPT_EXECUTION
                                WHERE SCRIPTID = :id",

                BindByName = true
            };
            command.Parameters.Add("id", id);
            command.CommandType = CommandType.Text;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var model = new ScriptExecutionModel();
                PopulateModel(reader, model);

                yield return model;
            }
        }

        public IEnumerable<ScriptAuditModel> LoadScriptAudit(int id)
        {
            using var command = new OracleCommand
            {
                Connection = DBContext.Connection,
                CommandText = @"SELECT 
                                    ID,
                                    NAME,
                                    DESCRIPTION,
                                    SCRIPT,
                                    OWNERID,
                                    LANGUAGE,
                                    SECURITYPERMISSION,
                                    ICON,
                                    DATE_MODIF,
                                    VERSION,
                                    MODIF,
                                    USERID
                                FROM RXD_AUDIT_SCRIPT
                                WHERE ID = :id",

                BindByName = true
            };
            command.Parameters.Add("id", id);
            command.CommandType = CommandType.Text;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var model = new ScriptAuditModel();
                PopulateModel(reader, model);

                yield return model;
            }
        }

        public IEnumerable<ScriptTriggerAuditModel> LoadTriggerAudit(int scriptId)
        {
            using var command = new OracleCommand
            {
                Connection = DBContext.Connection,
                CommandText = @"SELECT 
                                    ID,
                                    TIME,
                                    SCRIPTID,
                                    TRIGGERID,
                                    DATE_MODIF,
                                    VERSION,
                                    MODIF,
                                    USERID
                                FROM RXD_AUDIT_SCRIPT_TRIGGER
                                WHERE SCRIPTID = :scriptId",

                BindByName = true
            };
            command.Parameters.Add("scriptId", scriptId);
            command.CommandType = CommandType.Text;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var model = new ScriptTriggerAuditModel();
                PopulateModel(reader, model);

                yield return model;
            }
        }

        public ScriptModel LoadScript(int id)
        {
            var _triggers = new Dictionary<int, List<ScriptTriggerModel>>();

            using (var command = new OracleCommand())
            {
                command.Connection = DBContext.Connection;
                command.CommandText = @"SELECT 
                                            t.ID, 
                                            t.TRIGGERID, 
                                            t.SCRIPTID,
                                            t.TIME
                                        FROM RXD_SCRIPT_TRIGGER t
                                        INNER JOIN RXD_SCRIPT f ON t.SCRIPTID = f.ID
                                        WHERE 
                                        t.ScriptId = :id
                                        AND
                                        (
                                            f.SECURITYPERMISSION = :firmSecurityPermissionId
                                            OR 
                                            f.OWNERID = :ownerid
                                            OR
                                            :hasManageFirmRight = 1
                                        )";

                command.CommandType = CommandType.Text;
                command.BindByName = true;
                command.Parameters.Add("id", id);
                command.Parameters.Add("ownerid", _userId);
                command.Parameters.Add("firmSecurityPermissionId", (int)SecurityPermission.Firm);
                command.Parameters.Add("hasManageFirmRight", Convert.ToInt32(_hasManageFirmRight));

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var model = new ScriptTriggerModel();
                    PopulateModel(reader, model);

                    if (!_triggers.ContainsKey(model.ScriptId))
                        _triggers.Add(model.ScriptId, new List<ScriptTriggerModel>());
                    _triggers[model.ScriptId].Add(model);
                }
            }

            using (var command = new OracleCommand())
            {
                command.Connection = DBContext.Connection;
                command.CommandText = @"SELECT  
                                            ID,
                                            NAME,
                                            DESCRIPTION,
                                            SCRIPT,
                                            OWNERID,
                                            LANGUAGE,
                                            SECURITYPERMISSION,
                                            ICON
                                        FROM RXD_SCRIPT f
                                        WHERE 
                                            ID = :id
                                            AND
                                            (
                                                f.SECURITYPERMISSION = :firmSecurityPermissionId
                                                OR
                                                OWNERID = :ownerid
                                                OR
                                                :hasManageFirmRight = 1
                                            )";

                command.CommandType = CommandType.Text;
                command.BindByName = true;
                command.Parameters.Add(new OracleParameter("id", id));
                command.Parameters.Add(new OracleParameter("ownerid", _userId));
                command.Parameters.Add("firmSecurityPermissionId", (int)SecurityPermission.Firm);
                command.Parameters.Add("hasManageFirmRight", Convert.ToInt32(_hasManageFirmRight));

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var model = new ScriptModel();
                    PopulateModel(reader, model);

                    if (_triggers.ContainsKey(model.Id))
                    {
                        foreach (var trigger in _triggers[model.Id])
                        {
                            model.Triggers.Add(trigger);
                        }
                    }

                    return model;
                }
            }

            return null;
        }

        public void SaveScript(ScriptModel model)
        {
            ScriptModel oldModel = null;
            if (model.Id != 0)
                oldModel = LoadScript(model.Id);

            bool UpdateScript()
            {
                using var command = new OracleCommand
                {
                    Connection = DBContext.Connection,
                    CommandText = @"UPDATE RXD_SCRIPT
                                            SET name = :name, 
                                                description = :description, 
                                                script = :script,
                                                language = :language,
                                                ownerid = :ownerid,
                                                securitypermission = :securitypermission,
                                                icon = :icon
                                            WHERE id = :id",

                    BindByName = true
                };
                command.Parameters.Add("id", model.Id);
                command.Parameters.Add("name", model.Name);
                command.Parameters.Add("description", model.Description);
                command.Parameters.Add("script", model.Script);
                command.Parameters.Add("language", (int)model.Language);
                command.Parameters.Add("ownerid", model.OwnerId);
                command.Parameters.Add("securitypermission", (int)model.SecurityPermission);
                command.Parameters.Add("icon", model.Icon);

                command.CommandType = CommandType.Text;
                return (command.ExecuteNonQuery() != 0);
            }

            static bool UpdateTrigger(ScriptTriggerModel tm)
            {
                using var command = new OracleCommand();
                command.Connection = DBContext.Connection;
                command.CommandText = @"UPDATE RXD_SCRIPT_TRIGGER
                                            SET triggerId = :triggerId, 
                                                scriptId = :scriptId,
                                                time = :time
                                            WHERE id = :id";

                command.BindByName = true;
                command.Parameters.Add("id", tm.Id);
                command.Parameters.Add("triggerId", (int)tm.Trigger);
                command.Parameters.Add("scriptId", tm.ScriptId);
                command.Parameters.Add("time", tm.Time);

                command.CommandType = CommandType.Text;
                return (command.ExecuteNonQuery() != 0);
            }

            void InsertScript()
            {
                using var command = new OracleCommand();
                command.Connection = DBContext.Connection;
                command.CommandText = @"INSERT INTO RXD_SCRIPT (id, name, description, script, language, ownerid, securitypermission, icon)  
                                            VALUES(RXD_SCRIPT_ID.nextval, :name, :description, :script, :language, :ownerid, :securitypermission, :icon)
                                            RETURNING id into :id";

                command.BindByName = true;
                var parameter = command.Parameters.Add("id", model.Id);
                parameter.Direction = ParameterDirection.Output;

                command.Parameters.Add("name", model.Name);
                command.Parameters.Add("description", model.Description);
                command.Parameters.Add("script", model.Script);
                command.Parameters.Add("language", (int)model.Language);
                command.Parameters.Add("ownerid", model.OwnerId);
                command.Parameters.Add("securitypermission", (int)model.SecurityPermission);
                command.Parameters.Add("icon", model.Icon);

                command.CommandType = CommandType.Text;

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    throw new ApplicationException($"Expecting one row inserted. Got {rowsAffected}");
                }

                model.Id = Convert.ToInt32(parameter.Value);
            }

            void InsertTrigger(ScriptTriggerModel tm)
            {
                using var command = new OracleCommand();
                command.Connection = DBContext.Connection;
                command.CommandText = @"INSERT INTO RXD_SCRIPT_TRIGGER (id, triggerId, scriptId, time)  
                                            VALUES(RXD_SCRIPT_TRIGGER_ID.nextval, :triggerId, :scriptId, :time)
                                            RETURNING id into :id";

                command.BindByName = true;
                var parameter = command.Parameters.Add("id", model.Id);
                parameter.Direction = ParameterDirection.Output;

                command.Parameters.Add("triggerId", (int)tm.Trigger);
                command.Parameters.Add("scriptId", tm.ScriptId);
                command.Parameters.Add("time", tm.Time);

                command.CommandType = CommandType.Text;

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    throw new ApplicationException($"Expecting one row inserted. Got {rowsAffected}");
                }

                tm.Id = Convert.ToInt32(parameter.Value);
            }

            static void DeleteTrigger(int id)
            {
                using var command = new OracleCommand();
                command.Connection = DBContext.Connection;
                command.CommandText = @"DELETE FROM RXD_SCRIPT_TRIGGER  
                                            WHERE Id = :id";

                command.BindByName = true;
                command.Parameters.Add("id", id);
                command.CommandType = CommandType.Text;

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    throw new ApplicationException($"Expecting one row deleted. Got {rowsAffected}");
                }
            }

            var triggersToRemove = new List<int>();

            //Update the triggers
            if (oldModel != null)
            {
                //Find any deleted triggers and remove from the db
                foreach (var oldTrg in oldModel.Triggers)
                {
                    bool found = false;
                    foreach (var newTrg in model.Triggers)
                    {
                        if (newTrg.Id == oldTrg.Id)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        triggersToRemove.Add(oldTrg.Id);
                    }
                }
            }

            using var trans = DBContext.Connection.BeginTransaction();
            if (!UpdateScript())
            {
                InsertScript();
            }

            foreach (var trigger in triggersToRemove)
            {
                DeleteTrigger(trigger);
            }

            foreach (var trigger in model.Triggers)
            {
                trigger.ScriptId = model.Id;

                if (!UpdateTrigger(trigger))
                {
                    InsertTrigger(trigger);
                }
            }

            trans.Commit();
        }

        public void DeleteScript(int id)
        {
            using var command = new OracleCommand();
            command.Connection = DBContext.Connection;
            command.CommandText = "DELETE FROM RXD_SCRIPT WHERE ID = :id";
            command.CommandType = CommandType.Text;
            command.Parameters.Add(new OracleParameter("id", id));

            var rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
                throw new ApplicationException($"Unable to delete {id}");
        }

        public IEnumerable<UserModel> LoadUsers()
        {
            using var command = new OracleCommand();
            command.Connection = DBContext.Connection;
            command.CommandText = @"SELECT IDENT, NAME FROM RISKUSERS WHERE TYPE = 'U'";

            command.CommandType = CommandType.Text;
            command.BindByName = true;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var model = new UserModel();
                    model.Id = reader.GetInt32(reader.GetOrdinal("IDENT"));
                    model.Name = reader.GetString(reader.GetOrdinal("NAME"));

                    yield return model;
                }
            }
        }

        private void PopulateModel(OracleDataReader reader, ScriptModel model)
        {
            model.Id = reader.GetInt32(reader.GetOrdinal("ID"));
            model.Name = reader.GetString(reader.GetOrdinal("NAME"));

            if(!reader.IsDBNull(reader.GetOrdinal("DESCRIPTION")))
                model.Description = reader.GetString(reader.GetOrdinal("DESCRIPTION"));

            model.Script = reader.GetString(reader.GetOrdinal("SCRIPT"));
            model.Language = (Language)reader.GetInt32(reader.GetOrdinal("LANGUAGE"));
            model.OwnerId = reader.GetInt32(reader.GetOrdinal("OWNERID"));
            model.SecurityPermission = (SecurityPermission)reader.GetInt32(reader.GetOrdinal("SECURITYPERMISSION"));

            if (!reader.IsDBNull(reader.GetOrdinal("ICON")))
                model.Icon = reader.GetString(reader.GetOrdinal("ICON"));
        }

        private void PopulateModel(OracleDataReader reader, ScriptExecutionModel model)
        {
            model.Id = reader.GetInt32(reader.GetOrdinal("ID"));
            model.ScriptId = reader.GetInt32(reader.GetOrdinal("SCRIPTID"));
            model.StartedAt = reader.GetDateTime(reader.GetOrdinal("STARTEDAT"));
            model.EndedAt = reader.GetDateTime(reader.GetOrdinal("ENDEDAT"));
            model.Trigger = (Trigger)reader.GetInt32(reader.GetOrdinal("TRIGGERID"));

            if(!reader.IsDBNull(reader.GetOrdinal("RESULTS")))
                model.Results = reader.GetString(reader.GetOrdinal("RESULTS"));

            model.Status = (ExecutionStatus)reader.GetInt32(reader.GetOrdinal("STATUS"));
        }

        private void PopulateModel(OracleDataReader reader, ScriptAuditModel model)
        {
            PopulateModel(reader, model as ScriptModel);

            model.DateModified = reader.GetDateTime(reader.GetOrdinal("DATE_MODIF"));
            model.Version = reader.GetInt32(reader.GetOrdinal("VERSION"));
            model.Modification= reader.GetInt32(reader.GetOrdinal("MODIF"));
            model.UserId = reader.GetInt32(reader.GetOrdinal("USERID"));
        }

        private void PopulateModel(OracleDataReader reader, ScriptTriggerModel model)
        {
            model.ScriptId = reader.GetInt32(reader.GetOrdinal("SCRIPTID"));
            model.Trigger = (Trigger)reader.GetInt32(reader.GetOrdinal("TRIGGERID"));

            var timeIdx = reader.GetOrdinal("TIME");
            if (!reader.IsDBNull(timeIdx))
                model.Time = reader.GetDateTime(timeIdx);

            model.Id = reader.GetInt32(reader.GetOrdinal("ID"));
        }

        private void PopulateModel(OracleDataReader reader, ScriptTriggerAuditModel model)
        {
            PopulateModel(reader, model as ScriptTriggerModel);

            model.DateModified = reader.GetDateTime(reader.GetOrdinal("DATE_MODIF"));
            model.Version = reader.GetInt32(reader.GetOrdinal("VERSION"));
            model.Modification = reader.GetInt32(reader.GetOrdinal("MODIF"));
            model.UserId = reader.GetInt32(reader.GetOrdinal("USERID"));
        }
    }
}
