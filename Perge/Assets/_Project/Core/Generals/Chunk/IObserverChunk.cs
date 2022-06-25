namespace Core.Chunks
{
    public interface IObserverChunk
    {
        public void Subscribe(ISubscriberChunk subscriber);
        public void Unsubscribe(ISubscriberChunk subscriber);
    }
}