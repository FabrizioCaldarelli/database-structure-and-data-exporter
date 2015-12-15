using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;


namespace ___TOKEN_NAMESPACE___
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #region API
        protected void jsonRequestGET(String url, Dictionary<String, String> parameters, Action<ApiRequestCallback> callback)
        {
            HttpWebRequest request =  (HttpWebRequest)HttpWebRequest.Create(url);

            request.BeginGetResponse((IAsyncResult result) =>
            {
                ApiRequestCallback reqCallback = new ApiRequestCallback();
                try
                {
                    WebResponse response = request.EndGetResponse(result);
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string strJson = reader.ReadToEnd(); 
                        reqCallback.responseData = strJson;
                        reqCallback.requestSuccess = true;
                    }
                }
                catch (WebException e)
                {
                    reqCallback.requestSuccess = false;
                    reqCallback.operationError = e;
                }

                callback(reqCallback);
            }
            , request);
        }
        #endregion
    }
}
