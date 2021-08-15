namespace MediatorPattern
{
    public interface IRequest<out TResponse>
    {
    }

    public interface IHandler<in TRequest, out TResponse> where TRequest:IRequest<TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IMediator
    {
        T Send<T>(IRequest<T> request);
    }
}
