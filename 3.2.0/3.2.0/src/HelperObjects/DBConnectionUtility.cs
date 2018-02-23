using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNFMVC5Demo.HelperObjects
{
    public class DBConnectionUtility
    {
        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> data)
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["KNFConnection"].ConnectionString))
                {
                    await connection.OpenAsync(); // Asynchronously open a connection to the database
                    return await data(connection); // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

        protected T WithConnection<T>(Func<IDbConnection, T> data)
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["KNFConnection"].ConnectionString))
                {
                    connection.Open(); // Asynchronously open a connection to the database
                    return data(connection); // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

    }
}
