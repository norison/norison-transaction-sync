using MediatR;

namespace Norison.BankNotionConnector.Application.Features.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommandHandler : IRequestHandler<ProcessMonoWebHookDataCommand>
{
    public async Task Handle(ProcessMonoWebHookDataCommand request, CancellationToken cancellationToken)
    {
        
    }
}