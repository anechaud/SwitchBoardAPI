using System;
namespace SwitchBoardApi.Core.Model
{
	public class PaginationMetadata<T>
	{
		public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool PreviousPage { get; set; }
        public bool NextPage { get; set; }
        public List<T> ListOfItems { get; set; } = new List<T>();
    }
}