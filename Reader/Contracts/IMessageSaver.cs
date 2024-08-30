using Shared.Dtos;

namespace Reader.Contracts
{
    internal interface IMessageSaver
    {
        public void Save(MessageDto messageDto);
    }
}