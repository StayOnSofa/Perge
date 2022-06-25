using System;
using System.Collections;
using System.Threading;
using Core.Mono;

public class MonoThreading : MonoSingleton<MonoThreading>
{
   public static void Thread(Action actionThread)
   {
      MonoCoroutine.Instance.Play
         (Instance.ThreadAsync(actionThread));
   }

   public IEnumerator ThreadAsync(Action actionThread)
   {
      bool inJob = true;

      ThreadPool.QueueUserWorkItem((c) =>
      {
         actionThread?.Invoke();
         inJob = false;
      });
      
      while (inJob)
      {
         yield return null;
      }
      
      yield return null;
   }
}
