using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholderBot
{
    public class Messenger
    {

        private Dictionary<long, string> _usersCommand = new Dictionary<long, string>();
        private Dictionary<long, string> _traces = new Dictionary<long, string>();

        private Dictionary<long, int> _traceStage = new Dictionary<long, int>();
        private Tracer _tracer;
        private DatabaseReader _databaseReader;

        public Messenger(Tracer tracer)
        {
            _tracer = tracer;
        }

        public void Escape(long chatId)
        {
            if (_usersCommand.ContainsKey(chatId))
            {
                _usersCommand.Remove(chatId);
            }

            if (_traces.ContainsKey(chatId))
            {
                _traces.Remove(chatId);
            }

            if (_traceStage.ContainsKey(chatId))
            {
                _traceStage.Remove(chatId);
            }
        }
        
        public void TraceNew()
        {

        }
    }
}
