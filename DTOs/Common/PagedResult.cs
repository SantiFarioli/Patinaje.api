using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Common;

public record PagedResult<T>(int TotalItems, int Page, int PageSize, IEnumerable<T> Data);
