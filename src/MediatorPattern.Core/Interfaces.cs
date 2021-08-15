using System;

namespace MediatorPattern
{
    public interface IRequest<out TResponse>
    {
    }

    public interface IHandler<in TRequest, out TResponse> where TRequest : IRequest<TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IMediator
    {
        void Register(Type request, Type handler);
        T Send<T>(IRequest<T> request);
    }
}
