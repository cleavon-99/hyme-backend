using FluentResults;

namespace Hyme.Application.Errors
{
    public class ValidationError : Error
    {
        public string PropertyName { get; private set; }

        public ValidationError(string propertyName, string message) :base(message) 
        {
            PropertyName = propertyName;
        }
    }
}
