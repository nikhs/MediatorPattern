using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MediatorPattern.Core
{
    public class NaiveMediator : IMediator
    {
        ConcurrentDictionary<Type, Type> requestHandlerMap;

        public void Register(Type request, Type handler)
        {
            if (requestHandlerMap == null)
            {
                requestHandlerMap = new ConcurrentDictionary<Type, Type>();
            }
            _ = requestHandlerMap.TryAdd(request, handler);
        }

        public T Send<T>(IRequest<T> request)
        {
            if (requestHandlerMap == null || requestHandlerMap.Count == 0)
            {
                throw new ArgumentNullException($"Mediator does not have ANY handlers!");
            }

            var requestType = request.GetType();
            if (!requestHandlerMap.ContainsKey(requestType))
            {
                throw new ArgumentException($"Mediator does not have handlers for type `{requestType}`!");
            }

            var handlerType = requestHandlerMap[requestType];
            if (handlerType == null)
            {
                throw new ArgumentException($"Empty handler for type `{requestType}`!");
            }

            if (!handlerType.IsClass)
            {
                throw new ArgumentException($"Handler type - `{handlerType}` for request type `{requestType}` is not a class!");
            }

            const string handlerInterfaceName = "IHandler`2";
            const string requestInterfaceName = "IRequest`1";

            Type responseType = requestType.GetInterface(requestInterfaceName).GetGenericArguments()[0];
            var hasInterface = handlerType.FindInterfaces(
                    (interfaceType, criteria) =>
                    {
                        var hasCorrectInterface = interfaceType.IsInterface &&
                        interfaceType.IsGenericType &&
                        interfaceType.Name.Equals(handlerInterfaceName);
                        if (!hasCorrectInterface) return false;

                        var genericArgs = interfaceType.GetGenericArguments();
                        var doGenericParamsMatch = genericArgs[0] == requestType &&
                        genericArgs[1] == responseType;

                        return doGenericParamsMatch && hasCorrectInterface;
                    },
                    null
                ).Length > 0;

            if (!hasInterface)
            {
                throw new ArgumentException($"Handler type - `{handlerType}` for request type `{requestType}` is not a proper handler!");
            }

            object handlerInstance = Activator.CreateInstance(handlerType);
            return (T)handlerType.GetMethod("Handle").Invoke(handlerInstance, new object[] { request });
        }
    }
}
