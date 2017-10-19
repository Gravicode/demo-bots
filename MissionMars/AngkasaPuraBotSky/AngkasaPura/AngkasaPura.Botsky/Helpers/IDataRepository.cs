using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AngkasaPura.Botsky.Helpers
{
    public interface IDataRepository
    {
        string DatabaseName { set; get; }
        List<T> GetDataById<T>(string CollectionName, string ID);
        List<T> GetDataByQuery<T>(string CollectionName, string Query);
        Task<bool> InsertDoc<T>(string CollectionName, T data);
    } 
}