public static List<PermissionInfo> GetUserList()
        {
            using (var conn = db.GetConnection())
            {
                using (var cmd = db.GetSqlCommand(conn, "select * from cf_user"))
                {
                    return Entity<PermissionInfo>.SetValues(cmd.ExecuteReader());
                }
            }
        }
   using (var conn = db.GetConnection())
            {
                using (var cmd = db.GetSqlCommand(conn, "select emp_code,sum(setconfig) as sum_setconfig,sum(dataimport) as sum_dataimport from za where emp_code=@empcode"))
                {
                    db.AddParameter(cmd, "@empcode", empcode);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PermissionUser item = new PermissionUser();
                            item.EmpCode = Convert.ToString(reader.GetValue(0));
                            item.Config = Convert.ToInt32(reader.GetValue(1)) > 0 ? 1 : 0;
                            item.DataImport = Convert.ToInt32(reader.GetValue(2)) > 0 ? 1 : 0;
                            return item;
                        }
                    }
                }
            }
            using (var conn = db.GetConnection())
            {
                using (var cmd = db.GetStoredProcCommand(conn, "sp_web_"))
                {
                    return Entity<ObjectPerCostInfo>.SetValues(cmd.ExecuteReader());
                }
            }
          public static bool SubmitData()
        {
            bool result = false;
            using (var conn = db.GetConnection())
            {
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = db.GetSqlCommand(conn, @"insert into ma(object_name,sbu,
select objeata_time from tmp_data_details;"))
                    {
                        try
                        {
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                            result = true;
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
                if (result)
                {
                    using (var cmd = db.GetSqlCommand(conn, "delete from tmp_data_details"))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return result;
        }
