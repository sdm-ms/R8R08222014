using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace R8R.Core
{
    /// <summary>
    /// Repository
    /// </summary>
    public partial interface IRepository<T> where T : BaseEntity
    {
        T GetById(object id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> SprocExecuteList<T>(string storeprocName, params object[] sqlDbParam) where T : BaseEntity, new();
        IQueryable<T> Table { get; }
        int ExecuteStoredProcedureNonQuery(string commandText, params object[] parameters);

        #region Output Parametes
        /// <summary>
        /// Execute StoredProcedure List with Output parameters
        /// </summary>
        /// <typeparam name="T">TEntity</typeparam>
        /// <param name="storedProcedureName">Command Text or store procedure name</param>
        /// <param name="totalOutputParams">Total number of Output parameters</param>
        /// <param name="output">out object[] outputs</param>
        /// <param name="sqlDbParam">params object[] DBParameters</param>
        /// <returns>IQueryable TEntity</returns>
        IQueryable<T> StoredProcedureExecuteListWithOutput<T>(string storedProcedureName, int totalOutputParams, out object[] output, params object[] sqlDbParam) where T : BaseEntity, new();

        /// <summary>
        /// Execute Stored Procedure Non Query with Output parameters
        /// </summary>
        /// <param name="commandText">Command Text or store procedure name</param>
        /// <param name="totalOutputParams">Total number of Output parameters</param>
        /// <param name="outputs">out object[] outputs</param>
        /// <param name="parameters">params object[] parameters</param>
        /// <returns>Integer value</returns>
        int ExecuteStoredProcedureNonQueryWithOutput(string commandText, int totalOutputParams, out object[] outputs, params object[] parameters);
        #endregion
    }
}
