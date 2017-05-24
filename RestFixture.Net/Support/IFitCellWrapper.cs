using fit;

namespace RestFixture.Net.Support
{
    /// <summary>
    /// Wrapper of a Fit cell.
    /// </summary>
    public interface IFitCellWrapper : ICellWrapper
    {
        /// <returns> the underlying cell object. </returns>
        Parse Wrapped { get; }
    }
}