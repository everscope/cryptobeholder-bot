using CryptoBeholder.Lib.Interfaces;
using CryptoBeholderBot;
using Microsoft.EntityFrameworkCore;

namespace CryptoBeholder.DAL
{
    public class DatabaseReader : IDatabaseReader
    {
        private readonly UserContext _context;

        public DatabaseReader(UserContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _context.Database.EnsureCreated();
        }

        public void AddUser(long chatId)
        {
            if (!_context.Users.Any(p => p.ChatId == (int)chatId))
            {
                _context.Users.Add(new User() { ChatId = (int)chatId });
                _context.SaveChanges();
            }
        }

        public List<TrackedCoin> GetTrackedCoins(long chatId)
        {
            return _context.Users.First(p => p.ChatId == chatId).TrackedCoins.ToList();
        }

        public void AddTracketCoin(long id, string coinName)
        {
            var user = _context.Users.First(p => p.ChatId == id);

            if (user.TrackedCoins.Any(p => p.Coin == coinName))
            {
                throw new ArgumentException();
            }

            user.TrackedCoins.Add(new TrackedCoin() { Coin = coinName });

            _context.SaveChanges();
        }

        public void RemoveTrackedCoin(long id, string coinName)
        {
            var user = _context.Users.First(p => p.ChatId == id);
            var coin = user.TrackedCoins.First(p => p.Coin.ToLower() == coinName.ToLower());
            user.TrackedCoins.Remove(coin);

            _context.SaveChanges();
        }

        public bool IsCoinTracked(long id, string coinName)
        {
            return _context.Users.First(p => p.ChatId == id).TrackedCoins.
                Any(p => p.Coin.ToLower() == coinName.ToLower());
        }

        public string GetCoinName(long id, string coinName)
        {
            return _context.Users.First(p => p.ChatId == id).TrackedCoins.
                First(p => p.Coin.ToLower() == coinName.ToLower()).Coin;
        }

        public void ChangeVsCurrency(long id, string vsCurrency)
        {
            _context.Users.First(p => p.ChatId == id).VsCurrency = vsCurrency.ToLower();
            _context.SaveChanges();
        }

        public TraceMode GetTraceMode(long id, string coinName)
        {
            return _context.Users.First(p => p.ChatId == id).TrackedCoins
                .First(p => p.Coin.ToLower() == coinName.ToLower()).TraceSettings
                .TracingMode;
        }

        public void SetTraceMode(long id, string coinName, TraceMode traceMode)
        {
            _context.Users.First(p => p.ChatId == id).TrackedCoins
                .First(p => p.Coin == coinName).TraceSettings
                .TracingMode = traceMode;
            _context.SaveChanges();
        }

        public void SetMinValue(long id, string coinName, decimal min)
        {
            var traceSettings = _context.Users.First(p => p.ChatId == id).TrackedCoins
                .First(p => p.Coin == coinName).TraceSettings;
            traceSettings.MinIsReached = false;
            traceSettings.AbsoluteMin = min;
            _context.SaveChanges();
        }

        public void SetMaxValue(long id, string coinName, decimal max)
        {
            var traceSettings = _context.Users.First(p => p.ChatId == id).TrackedCoins
               .First(p => p.Coin == coinName).TraceSettings;
            traceSettings.MinIsReached = false;
            traceSettings.AbsoluteMin = max;
            _context.SaveChanges();
        }

        public void SetPercents(long id, string coinName, decimal percent)
        {
            var traceSettings = _context.Users.First(p => p.ChatId == id).TrackedCoins
               .First(p => p.Coin == coinName).TraceSettings;
            traceSettings.Persent = percent;
            traceSettings.PersentPositiveIsReached = false;
            traceSettings.PersentNegativeIsReached = false;
            _context.SaveChanges();
        }

        public void SetTime(long id, string coinName, DateTime dateTime)
        {
            var traceSettings = _context.Users.First(p => p.ChatId == id).TrackedCoins
               .First(p => p.Coin == coinName).TraceSettings;
            traceSettings.Time = dateTime;
            traceSettings.Timestamp = DateTime.Now;
            _context.SaveChanges();
        }

        public User GetUser(long id)
        {
            return _context.Users.First(p => p.ChatId == id);
        }

        public User[] GetAllUsers()
        {
            return _context.Users.AsNoTracking().ToArray();
        }

        public void SetAbsoluteMaxReached(long id, string coinName)
        {
            var traceSettings = _context.Users.First(p => p.ChatId == id)
                .TrackedCoins.First(p => p.Coin == coinName).TraceSettings;
            traceSettings.MaxIsReached = true;

            _context.SaveChanges();
        }

        public void SetAbsoluteMinReached(long id, string coinName)
        {
            var traceSettings = _context.Users.First(p => p.ChatId == id)
                .TrackedCoins.First(p => p.Coin == coinName).TraceSettings;
            traceSettings.MaxIsReached = true;

            _context.SaveChanges();
        }

        public void SetPercentMinReached(long id, string coinName)
        {
            var traceSettings = _context.Users.First(p => p.ChatId == id)
                .TrackedCoins.First(p => p.Coin == coinName).TraceSettings;
            traceSettings.PersentNegativeIsReached = true;

            _context.SaveChanges();
        }
        public void SetPercentMaxReached(long id, string coinName)
        {
            var traceSettings = _context.Users.First(p => p.ChatId == id)
                .TrackedCoins.First(p => p.Coin == coinName).TraceSettings;
            traceSettings.PersentPositiveIsReached = true;

            _context.SaveChanges();
        }

        public void SetTimeStamp(long id, string coinName)
        {
            var traceSettings = _context.Users.First(p => p.ChatId == id)
                .TrackedCoins.First(p => p.Coin == coinName).TraceSettings;
            traceSettings.Timestamp = DateTime.Now;

            _context.SaveChanges();
        }

    }
}
