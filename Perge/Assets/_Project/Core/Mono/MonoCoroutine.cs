using System.Collections;

namespace Core.Mono
{
    public class MonoCoroutine : MonoSingleton<MonoCoroutine>
    {
        public void Play(IEnumerator action)
        {
            StartCoroutine(action);
        }
    }
}