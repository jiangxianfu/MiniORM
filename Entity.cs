    //新版本的EntityORM
    public abstract class BaseEntity
    {
        /// <summary>
        /// 抽象方法
        /// </summary>
        /// <param name="reader"></param>
        internal abstract void SetEntity(DbDataReader reader, Dictionary<string, int> lowerFields);
        /// <summary>
        /// 获取数据库字段数据
        /// </summary>
        /// <typeparam name="ObjectType"></typeparam>
        /// <param name="reader"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        internal ObjectType GetField<ObjectType>(DbDataReader reader, int fieldIndex)
        {
            if (reader.IsDBNull(fieldIndex))
                return default(ObjectType);
            object obj = reader.GetValue(fieldIndex);
            Type dtype = typeof(ObjectType);
            if (!dtype.IsGenericType)
                return (ObjectType)Convert.ChangeType(obj, dtype);
            if (dtype.GetGenericTypeDefinition() == typeof(Nullable<>))
                return (ObjectType)Convert.ChangeType(obj, Nullable.GetUnderlyingType(dtype));
            throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".", obj.GetType().FullName, dtype.FullName));
        }
    }

    public class Entity<T>
        where T : BaseEntity, new()
    {
        public static List<T> SetValues(DbDataReader reader)
        {
            try
            {
                List<T> list = new List<T>();
                Dictionary<string, int> lowerFields = new Dictionary<string, int>();
                bool first = true;
                while (reader.Read())
                {
                    if (first)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            lowerFields.Add(reader.GetName(i).ToLower(), i);
                        }
                        first = false;
                    }
                    T item = new T();
                    item.SetEntity(reader, lowerFields);
                    list.Add(item);
                }
                return list;
            }
            finally
            {
                reader.Close();
            }
        }
        public static List<T> UnSafeSetValues(DbDataReader reader)
        {
            List<T> list = new List<T>();
            Dictionary<string, int> lowerFields = new Dictionary<string, int>();
            bool first = true;
            while (reader.Read())
            {
                if (first)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        lowerFields.Add(reader.GetName(i).ToLower(), i);
                    }
                    first = false;
                }
                T item = new T();
                item.SetEntity(reader, lowerFields);
                list.Add(item);
            }
            return list;
        }
        public static T SetValue(DbDataReader reader)
        {
            try
            {
                Dictionary<string, int> lowerFieldIndex = new Dictionary<string, int>();
                bool first = true;
                if (reader.Read())
                {
                    if (first)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            lowerFieldIndex.Add(reader.GetName(i).Trim().ToLower(), i);
                        }
                        first = false;
                    }
                    T item = new T();
                    item.SetEntity(reader, lowerFieldIndex);
                    return item;
                }
            }
            finally
            {
                reader.Close();
            }
            return null;
        }
        public static T UnSafeSetValue(DbDataReader reader)
        {
            Dictionary<string, int> lowerFieldIndex = new Dictionary<string, int>();
            bool first = true;
            if (reader.Read())
            {
                if (first)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        lowerFieldIndex.Add(reader.GetName(i).Trim().ToLower(), i);
                    }
                    first = false;
                }
                T item = new T();
                item.SetEntity(reader, lowerFieldIndex);
                return item;
            }

            return null;
        }
    }
