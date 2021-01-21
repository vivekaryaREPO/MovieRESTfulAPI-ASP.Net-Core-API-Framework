using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private readonly int maxRecordsPerPage = 10;
        private int recordsPerPage = 10;
        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                recordsPerPage = (recordsPerPage > maxRecordsPerPage) ? maxRecordsPerPage : value;
            }
        }
    }
}
