using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;

namespace DevChat
{
    using MessageInfo = DSharpPlus.EventArgs.MessageCreateEventArgs;

    public static class MessageManager
    {
        public static MessageInfo GetMessageMatch(Func<MessageInfo, bool> predicate)
        {
            MessageInfo target = null;

            lock (s_lockList)
            {
                for (int m = 0; m < s_msgList.Count; ++m)
                {
                    if (predicate(s_msgList[m]))
                    {
                        target = s_msgList[m];

                        s_msgList.RemoveRange(0, m + 1);

                        break;
                    }
                }

                if (target == null)
                {
                    s_msgList.Clear();
                }
            }

            return target;
        }

        public static void PushMessage(MessageInfo msg)
        {
            lock (s_lockList)
            {
                s_msgList.Add(msg);

                if (s_msgList.Count > 128)
                {
                    s_msgList.RemoveAt(0);
                }
            }
        }

        public static void Clear()
        {
            lock (s_lockList)
            {
                s_msgList.Clear();
            }
        }

        private static List<MessageInfo> s_msgList = new List<MessageInfo>();
        private static readonly s_lockList = new object();
    }
}
