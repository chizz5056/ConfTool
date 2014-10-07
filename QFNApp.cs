using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickFix;


namespace LSEHub.ConfTool
{
    public class QFNApp : IApplication
    {
        SessionSettings settings;
        IMessageStoreFactory storeFactory;
        ILogFactory logFactory;
        QuickFix.Transport.SocketInitiator initiator;
        SessionID qfnSessionID { get; set; }
        public Boolean IsConnected { get; set; }
        public event Action<QuickFix.Message, bool> MessageEvent;



        public QFNApp()
        {
            settings = new SessionSettings("initiator.cfg");           
            //QFNApp
            storeFactory = new FileStoreFactory(settings);
            logFactory = new FileLogFactory(settings);
            initiator = new QuickFix.Transport.SocketInitiator(this, storeFactory, settings, logFactory); 
        }

        public QFNApp(SessionSettings ss)
        {
            settings = ss;
            //QFNApp
            storeFactory = new FileStoreFactory(settings);
            logFactory = new FileLogFactory(settings);
            initiator = new QuickFix.Transport.SocketInitiator(this, storeFactory, settings, logFactory); 
        }

        public void Start()
        {
            if (initiator.IsStopped)
                initiator.Start();
        }

        public void Send(RawMessage message)
        {
            //RawToQFNGenerator.QFNMessage(message);
            if (IsConnected)
                Session.SendToTarget(RawToQFNGenerator.QFNMessage(message));
        }

        public void FromAdmin(Message message, SessionID sessionID)
        {
            //throw new NotImplementedException();
            //Console.WriteLine("FromAdmin: {0} {1}", sessionID.ToString(), message.ToString());
        }

        public void FromApp(Message message, SessionID sessionID)
        {
            //throw new NotImplementedException();
            //Console.WriteLine("FromApp: {0} {1}", sessionID.ToString(), message.ToString());
            
            if (MessageEvent != null)
                MessageEvent(message, true);

        }

        public void OnCreate(SessionID sessionID)
        {
            //throw new NotImplementedException();
            //Console.WriteLine("OnCreate: {0}", sessionID.ToString());
            qfnSessionID = sessionID;
        }

        public void OnLogon(SessionID sessionID)
        {
            //throw new NotImplementedException();
            //Console.WriteLine("OnLogon: {0}", sessionID.ToString());
            IsConnected = true;
        }

        public void OnLogout(SessionID sessionID)
        {
            //throw new NotImplementedException();
            //Console.WriteLine("OnLogout: {0}", sessionID.ToString());
            IsConnected = false;
        }

        public void ToAdmin(Message message, SessionID sessionID)
        {
            //throw new NotImplementedException();
            //Console.WriteLine("ToAdmin: {0} {1}", sessionID.ToString(), message.ToString());

        }

        public void ToApp(Message message, SessionID sessionId)
        {
            //throw new NotImplementedException();
            //Console.WriteLine("ToApp: {0} {1}", sessionId.ToString(), message.ToString());
        }
    }
}
