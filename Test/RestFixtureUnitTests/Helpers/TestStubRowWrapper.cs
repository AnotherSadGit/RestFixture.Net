using System;
using Moq;
using RestFixture.Net.TableElements;

namespace RestFixture.Net.UnitTests.Helpers
{
    public class TestStubRowWrapper : IRowWrapper<string>
    {
        private readonly string[] _cells;

        public TestStubRowWrapper(params string[] cells)
        {
            _cells = cells;
        }

        /// <param name="c"> the cell index </param>
        /// <returns> the <seealso cref="ICellWrapper{T}"/> at a given position </returns>
        public ICellWrapper<string> getCell(int c)
        {
            ICellWrapper<string> cellWrapper =
                    Mock.Of<ICellWrapper<string>>(wrapper =>
                        wrapper.Wrapped == _cells[c] &&
                        wrapper.text() == _cells[c] &&
                        wrapper.body() == _cells[c]);
            return cellWrapper;
        }

        /// <returns> the row size. </returns>
        public int size()
        {
            return _cells.Length;
        }

        /// <summary>
        /// removes a cell at a given position.
        /// </summary>
        /// <param name="c"> the cell index </param>
        /// <returns> the removed cell. </returns>
        public ICellWrapper<string> removeCell(int c)
        {
            throw new NotImplementedException();
        }
    }
}