using System;
using System.Collections.Generic;
using System.Linq;
using System.Net ;
using System.Text;
using System.Threading ;
using System.Threading.Tasks ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.WatcherServer.Communication {
   public class HttpListenerCallbackState {
      public HttpListenerCallbackState (HttpListener listener) {
         if (listener == null) throw new ArgumentNullException (nameof (listener)) ;

         Listener = listener ;
         ListenForNextRequest = new AutoResetEvent (false) ;
      }

      public HttpListener Listener {get ;}

      public AutoResetEvent ListenForNextRequest {get ;}
   }

   public class HttpRequestHandler {
      private readonly ManualResetEvent _stopEvent = new ManualResetEvent (false) ;

      private Action<HttpListenerContext> _requestHandler ;

      public void ListenAsynchronously (IEnumerable<string> prefixes,
                                        Action<HttpListenerContext> requestHandler) {
         HttpListener listener = new HttpListener() ;

         _requestHandler = requestHandler ;

         foreach (string prefix in prefixes) {
            listener.Prefixes.Add (prefix) ;
         }

         Logger.Log ("Http listener started.") ;
         listener.Start() ;

         StartListening (listener) ;

         //// Try 4 more threads
         //StartListening (listener) ;
         //StartListening (listener) ;
         //StartListening (listener) ;
         //StartListening (listener) ;
      }

      private void StartListening (HttpListener listener) {
         HttpListenerCallbackState state = new HttpListenerCallbackState (listener) ;
         var task = new Task (Listen, state, TaskCreationOptions.LongRunning) ;
         task.Start() ;
      }

      public void StopListening() {
         _stopEvent.Set() ;
      }

      private void Listen (object state) {
         HttpListenerCallbackState callbackState = (HttpListenerCallbackState) state ;

         while (callbackState.Listener.IsListening) {
            callbackState.Listener.BeginGetContext (ListenerCallback, callbackState) ;

            bool stop = WaitHandle.WaitAny (new WaitHandle[] {callbackState.ListenForNextRequest, _stopEvent}) == 1 ;

            if (stop) {
               // stopEvent was signalled
               Logger.Log ("Http listener stopped.") ;
               callbackState.Listener.Stop() ;
               break ;
            }
         }
      }

      private void ListenerCallback (IAsyncResult asyncResult) {
         HttpListenerCallbackState callbackState = (HttpListenerCallbackState) asyncResult.AsyncState ;
         HttpListenerContext context = null ;

         try {
            if (callbackState.Listener.IsListening) {
               context = callbackState.Listener.EndGetContext (asyncResult) ;
            }
         } catch (Exception) {
            return ;
         } finally {
            callbackState.ListenForNextRequest.Set() ;
         }

         if (context == null) return ;

         _requestHandler (context) ;
      }
   }
}