using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.ChatRequests.Commands.SendChatRequest
{
   
        public record SendChatRequestCommand(string ToUserId) : IRequest<long>;
    
}
