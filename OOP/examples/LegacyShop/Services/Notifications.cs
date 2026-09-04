using System;

namespace LegacyShop.Services
{
    public class EmailNotifier
    {
        private readonly string _from;

        public EmailNotifier(string from)
        {
            _from = from;
        }

        public void Send(string address, string subject, string body)
        {
            Console.WriteLine("  [email] " + _from + " -> " + address + ": " + subject);
            Console.WriteLine("          " + body);
        }
    }

    public class SmsGateway
    {
        private readonly string _senderName;

        public SmsGateway(string senderName)
        {
            _senderName = senderName;
        }

        public bool Push(string phoneNumber, string text)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }
            Console.WriteLine("  [sms] " + _senderName + " -> " + phoneNumber + ": " + text);
            return true;
        }
    }
}
