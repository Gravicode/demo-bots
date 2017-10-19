﻿using AngkasaPura.Botsky.Business;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AngkasaPura.Botsky.Helpers
{
    public class RedisDB : IDataRepository
    {
        private readonly PooledRedisClientManager redisManager;
        string ConStr = "vFfVFMQI5xC/Q4Ib4Y08mcrup6hNixMV8zYu7lqte4g=@redis-murahaje.redis.cache.windows.net:6379";
        public RedisDB()
        {
            redisManager = new PooledRedisClientManager(3, ConStr);
        }
        public string DatabaseName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<T> GetDataById<T>(string CollectionName, string ID)
        {
            return default(List<T>);
        }

        public List<T> GetDataByQuery<T>(string CollectionName, string Query)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertDoc<T>(string CollectionName, T data)
        {
            try
            {
                
                using (var redis = redisManager.GetClient())
                {
                    var redisTodos = redis.As<T>();
                    if (data is Complain)
                    {
                        var node = data as Complain;
                        node.Id = redisTodos.GetNextSequence();
                        redisTodos.Store(data);
                    }
                    else
                    {
                        redisTodos.Store(data);
                    }
                    
                   
                    return Task.FromResult(true);
                }
            }
            catch { return Task.FromResult(false); }
        }

        public List<T> GetAllData<T>()
        {
            using (var redis = redisManager.GetClient())
            {
                var redisTodos = redis.As<T>();
                return redisTodos.GetAll().ToList();
            }
        }
    }
}