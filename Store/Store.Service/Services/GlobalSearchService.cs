using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Common.Parameters;
using Store.Common.Parameters.Filtering;
using Store.Repository.Common.Core;
using Store.Service.Common.Services;

namespace Store.Services
{
    internal class GlobalSearchService : ParametersService, IGlobalSearchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GlobalSearchService
        (
            IUnitOfWork unitOfWork,
            IQueryUtilityFacade queryUtilityFacade
        ) : base (queryUtilityFacade)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ISearchItem>> FindAsync(IGlobalFilteringParameters filtering)
        {
            IList<ISearchItem> searchItems = new List<ISearchItem>();

            if (filtering.SearchTypes.Contains(SectionType.Book))
            {
                IEnumerable<IBook> books = await _unitOfWork.BookRepository.FindAsync
                (
                    filterExpression: b => b.Name.Contains(filtering.SearchString) || b.Author.Contains(filtering.SearchString) || b.Bookstore.Name.Contains(filtering.SearchString),
                    sorting: SortingFactory.Create(new[] { $"{nameof(IBook.Name)}|desc" }),
                    options: OptionsFactory.Create(new[] { nameof(IBook.Bookstore) })
                );

                foreach (IBook book in books)
                {
                    ISearchItem searchItem = new SearchItem
                    {
                        Id = book.Id,
                        Name = $"{book.Name} ({book.Bookstore.Name})",
                        Type = SectionType.Book
                    };

                    searchItems.Add(searchItem);
                }
            }

            if (filtering.SearchTypes.Contains(SectionType.Bookstore))
            {
                IEnumerable<IBookstore> bookstores = await _unitOfWork.BookstoreRepository.FindAsync
                (
                    filterExpression: bs => bs.Name.Contains(filtering.SearchString) || bs.Location.Contains(filtering.SearchString),
                    sorting: SortingFactory.Create(new[] { $"{nameof(IBookstore.Name)}|desc" }),
                    options: null
                );

                foreach (IBookstore bookstore in bookstores)
                {
                    ISearchItem searchItem = new SearchItem
                    {
                        Id = bookstore.Id,
                        Name = bookstore.Name,
                        Type = SectionType.Bookstore
                    };

                    searchItems.Add(searchItem);
                }
            }

            return searchItems;
        }
    }
}