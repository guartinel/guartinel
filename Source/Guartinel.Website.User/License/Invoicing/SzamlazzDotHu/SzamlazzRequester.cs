using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;
using Guartinel.Website.User.License.Invoicing.SzamlazzDotHu.DO;

namespace Guartinel.Website.User.License.Invoicing.SzamlazzDotHu {
   public class SzamlazzRequester {
      readonly HttpClient _client ;
      public SzamlazzRequester() {
         CookieContainer cookies = new CookieContainer();
         HttpClientHandler handler = new HttpClientHandler { CookieContainer = cookies, UseCookies = true, AllowAutoRedirect = false };
         _client = new HttpClient(handler);
      }

      public  SzamlaResponse.szamlavalasz CreateInvoice (SzamlaRequest.xmlszamla data) {  
         XmlSerializer requestSerializer = new XmlSerializer(typeof(SzamlaRequest.xmlszamla));
         StringBuilder stringBuilder = new StringBuilder();
         XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings {
            Encoding = Encoding.UTF8,
            Indent = true,
            OmitXmlDeclaration = true,
         });
         requestSerializer.Serialize(xmlWriter, data);
         string requestXMLString = stringBuilder.ToString();
         Logger.Log($"Request towards szamlazz.hu {requestXMLString}");
         byte[] bytes = Encoding.UTF8.GetBytes(requestXMLString);

         ByteArrayContent content = new ByteArrayContent(bytes);
         string text = content.ReadAsStringAsync().Result;

         using ( var form = new MultipartFormDataContent() ) {
            form.Add(content, "action-xmlagentxmlfile", "data.xml");
            HttpResponseMessage httpResponse = _client.PostAsync("https://www.szamlazz.hu/szamla/", form).Result;

            if ( !httpResponse.IsSuccessStatusCode ) {
               throw new Exception($"Cannot call szamlazz.hu, connection error.HTTPStatusCode: {httpResponse.StatusCode} {httpResponse.Content}");
            }

            string stringResponse = httpResponse.Content.ReadAsStringAsync().Result;
            SzamlaResponse.szamlavalasz responseDeserialized = null;
            try {
               XmlSerializer responseSerializer = new XmlSerializer(typeof(SzamlaResponse.szamlavalasz));
               XmlReader xmlReader = XmlReader.Create(new StringReader(stringResponse));

             responseDeserialized = responseSerializer.Deserialize(xmlReader) as SzamlaResponse.szamlavalasz;
             
            } catch ( Exception ex ) {
               throw new Exception($"Error while deserializing the response Cause:{ex.GetAllMessages()}", ex);
            }

            if ( responseDeserialized == null || responseDeserialized.hibakod != null || responseDeserialized.hibauzenet != null ) {
               throw new Exception($"Error while creating invoice: {responseDeserialized.hibauzenet}, {responseDeserialized.hibakod}");
            }
            return responseDeserialized;
         }
      }
   }

}