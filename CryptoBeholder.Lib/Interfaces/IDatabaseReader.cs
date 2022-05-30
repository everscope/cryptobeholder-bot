using CryptoBeholderBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholder.Lib.Interfaces
{
    public interface IDatabaseReader
    {
        public void Initialize();
        public void AddUser(long chatId);
        public List<TrackedCoin> GetTrackedCoins(long chatId);
        public void AddTracketCoin(long id, string coinName);
        public void RemoveTrackedCoin(long id, string coinName);
        public bool IsCoinTracked(long id, string coinName);
        public string GetCoinName(long id, string coinName);
        public void ChangeVsCurrency(long id, string vsCurrency);
        public TraceMode GetTraceMode(long id, string coinName);
        public void SetTraceMode(long id, string coinName, TraceMode traceMode);
        public void SetMinValue(long id, string coinName, decimal min);
        public void SetMaxValue(long id, string coinName, decimal max);
        public void SetPercents(long id, string coinName, decimal percent);
        public void SetTime(long id, string coinName, DateTime dateTime);
        public User GetUser(long id);
        public User[] GetAllUsers();
        public void SetAbsoluteMaxReached(long id, string coinName);
        public void SetAbsoluteMinReached(long id, string coinName);
        public void SetPercentMinReached(long id, string coinName);
        public void SetPercentMaxReached(long id, string coinName);
        public void SetTimeStamp(long id, string coinName);

    }
}
