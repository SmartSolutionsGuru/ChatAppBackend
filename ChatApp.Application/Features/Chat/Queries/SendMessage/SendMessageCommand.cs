using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.SendMessage
{
    public record SendMessageCommand(
     long ChatId,
     string Content,
     bool IsVoiceNote = false,
     string? VoiceNoteUrl = null,
     double? VoiceNoteDuration = null,
     double[]? VoiceNoteWaveform = null
 ) : IRequest<long>;
}
