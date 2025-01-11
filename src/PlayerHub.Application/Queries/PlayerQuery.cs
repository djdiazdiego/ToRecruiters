using Core.Head.CQRS;
using Core.Head.Wrappers;
using Microsoft.AspNetCore.OData.Query;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Domain;

namespace PlayerHub.Application.Queries
{
    public sealed class PlayerQuery(ODataQueryOptions<Player> oData) : IQuery<PageResponse<PlayerDTO>>
    {
        public ODataQueryOptions<Player> OData { get; } = oData;
    }
}
