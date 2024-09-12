using MediatR;

namespace Norison.BankNotionConnector.Application.Features.Commands.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommandHandler : IRequestHandler<ProcessMonoWebHookDataCommand>
{
    public async Task Handle(ProcessMonoWebHookDataCommand request, CancellationToken cancellationToken)
    {
        
    }
}