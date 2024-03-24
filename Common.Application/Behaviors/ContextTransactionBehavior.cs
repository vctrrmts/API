using Common.Application.Abstractions.Persistence;
using MediatR;

namespace Common.Application.Behaviors
{
    public class ContextTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IContextTransactionCreator _contextTransactionCreator;

        public ContextTransactionBehavior(IContextTransactionCreator contextTransactionCreator) 
        {
            _contextTransactionCreator = contextTransactionCreator;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            using (var transaction = await _contextTransactionCreator.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var result = await next();
                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    throw;
                }
            }
        }
    }
}
