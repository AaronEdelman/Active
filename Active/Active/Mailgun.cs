using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Active
{
    public static class Mailgun
    {
        public static IRestResponse SendSimpleMessage(string receiverEmail, string receiverName, string senderFirstName, string senderLastName, string activity, string date, string senderEmail)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            "key-d9779d82c512ce48c9cbb6848e5442b2");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox1205fb1736974a9f9dd1a66a41870ea1.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "InterActive Team <mailgun@sandbox1205fb1736974a9f9dd1a66a41870ea1.mailgun.org>");
            request.AddParameter("to", receiverEmail);
            request.AddParameter("subject", "InterActive Message");
            request.AddParameter("text", "Hi "+receiverName+", you recently met up with "+senderFirstName+" "+senderLastName+" at "+activity+" on "+date+". Here is their Email in case you want to hang out again: "+ senderEmail);
            request.Method = Method.POST;
            return client.Execute(request);
        }
    }
}