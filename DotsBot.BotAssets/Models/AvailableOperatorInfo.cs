using System.Collections.Generic;
using System.Linq;

namespace DotsBot.Models
{
    public class GetAvailableOperatorsResponse
    {
        public GetAvailableOperatorsResponse()
        {
            AvailableOperators = new List<AvailableOperatorInfo>();
        }

        public IList<AvailableOperatorInfo> AvailableOperators { get; set; }

        public override string ToString()
        {
            return string.Join(", ", AvailableOperators.Select(ao => ao.ToString()));
        }
    }

    public class AvailableOperatorInfo
    {
        public string DisplayName { get; set; }
        public int UserId { get; set; }

        public override string ToString()
        {
            return $"{nameof(DisplayName)}: {DisplayName}, {nameof(UserId)}: {UserId}";
        }
    }
}