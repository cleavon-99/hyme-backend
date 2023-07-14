using FluentResults;
using Hyme.Application.DTOs.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Commands.Authentication
{
    public record ConnectToWalletCommand(string Message, string Signature, string WalletAddress) : IRequest<Result<AuthenticationResponse>>;
 
}
