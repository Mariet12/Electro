using Electro.Core.Dtos.Search;

namespace Electro.Core.Interface
{
    public interface ISearchService
    {
        Task<PagedResult<SearchHitDto>> SearchAsync(
         SearchQueryDto query,
         string? requesterUserId,   // ✅ nullable
         bool requesterIsAdmin,
         CancellationToken ct = default);
    }
}
